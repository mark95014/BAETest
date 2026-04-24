using LDSTest.Shared;
using Microsoft.Playwright;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace LDSUITest.Hooks
{
    [Binding]
    public class UITestHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _browserContext;
        private IPage _page = null!;

        private TestRail _testRail = new();

        public UITestHooks(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        [BeforeFeature]
        public static async Task BeforeFeature(FeatureContext featureContext)
        {
            await new Database().ResetDatabase();
        }

        [AfterFeature]
        public static async Task AfterFeature(FeatureContext featureContext)
        {
            await new Database().ResetDatabase();
        }

        [BeforeScenario(Order = 0)]
        public async Task BeforeScenario()
        {
            try
            {
                InitializeTestMetadata();

                await InitializePlaywrightAndBrowser();
                InitializeResultsAndExpectedResults();

                StoreScenarioObjects();
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"[BEFORE SCENARIO ERROR] {ex.GetType().Name}: {ex.Message}");
                TestContext.Progress.WriteLine($"[BEFORE SCENARIO ERROR] Stack: {ex.StackTrace}");
                throw;
            }
        }

        [AfterScenario(Order = 100)]
        public async Task AfterScenario()
        {
            try
            {
                await CleanupBrowser();

                var results = _scenarioContext.Get<Results>("Results");
                var expectedResults = _scenarioContext.Get<ExpectedResults>("ExpectedResults");

                await CaptureFailureScreenshotIfNeeded();

                results.Display();
                string errorMessages = results.GetErrorMessages() ?? "";

                if (results.HasFailures())
                {
                    _testRail?.AddUnSuccessfulTestRailResult(errorMessages);
                    Assert.Fail($"Scenario Failed: {errorMessages}");
                }
                else
                {
                    _testRail?.AddSuccessfulTestRailResult();
                }

                expectedResults.Close();
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"[AFTER SCENARIO ERROR] {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                TestNameProvider.Clear();
                TestCaseIdProvider.Clear();
            }
        }

        // -------------------------
        // PRIVATE HELPERS
        // -------------------------

        private int ExtractTestCaseId()
        {
            var tag = _scenarioContext.ScenarioInfo.Tags.FirstOrDefault(t => t.StartsWith("testcase:"));
            if (tag != null && int.TryParse(tag.Replace("testcase:", ""), out int id))
                return id;

            return 0;
        }

        private void InitializeTestMetadata()
        {
            TestCaseIdProvider.SetTestCaseId(ExtractTestCaseId());
            TestNameProvider.SetTestName(_featureContext.FeatureInfo.Title);
            _testRail = new TestRail();
        }

        private async Task InitializePlaywrightAndBrowser()
        {
            var slowMo = int.TryParse(TestContext.Parameters["slowMo"], out int slowMoValue) ? slowMoValue : 0;
            var headless = Boolean.Parse(TestContext.Parameters["headless"] ?? "true");

            _playwright = await Playwright.CreateAsync();

            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless,
                SlowMo = headless ? 0 : slowMo
            });

            _browserContext = await _browser.NewContextAsync();
            _page = await _browserContext.NewPageAsync();
        }

        private void InitializeResultsAndExpectedResults()
        {
            var expectedResultsFolder = TestContext.Parameters["expectedResultsFolder"] ?? "../../../data/expectedResults";
            var generateExpectedResults = Boolean.Parse(TestContext.Parameters["generateExpectedResults"] ?? "false");

            var results = new Results();
            var expectedResults = new ExpectedResults(TestNameProvider.GetTestName(), expectedResultsFolder, generateExpectedResults);
            expectedResults.Init();

            _scenarioContext["Results"] = results;
            _scenarioContext["ExpectedResults"] = expectedResults;
        }

        private void StoreScenarioObjects()
        {
            _scenarioContext["Page"] = _page;
        }

        private async Task CleanupBrowser()
        {
            if (_page != null && !_page.IsClosed) await _page.CloseAsync();
            if (_browserContext != null) await _browserContext.CloseAsync();
            if (_browser != null) await _browser.CloseAsync();
            _playwright?.Dispose();
        }

        private async Task CaptureFailureScreenshotIfNeeded()
        {
            if (_scenarioContext.TestError != null && _page != null)
            {
                var screenshotPath = Path.Combine("screenshots", $"{_featureContext.FeatureInfo.Title}.png");
                await _page.ScreenshotAsync(new() { Path = screenshotPath });
            }
        }
    }
}