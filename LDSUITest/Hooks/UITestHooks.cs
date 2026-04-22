using LDSTest.Shared;
using LDSUITest.utils;
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
        private IBrowser? _browser;
        private IBrowserContext? _browserContext;
        
        private IPage _page = null!;
        private ExpectedResults _expectedResults = null!;
        private Results _results = null!;
        private TestRail _testRail = null!;
        private bool _generateExpectedResults;

        public UITestHooks(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
            
            TestContext.Progress.WriteLine($"[CONSTRUCTOR] UITestHooks created for scenario: {scenarioContext.ScenarioInfo.Title}");
        }

        [BeforeScenario(Order = 0)]
        public async Task BeforeScenario()
        {
            TestContext.Progress.WriteLine($"[BEFORE SCENARIO START] {_scenarioContext.ScenarioInfo.Title}");
            
            try
            {
                // 1. Get test case ID
                var testCaseTag = _scenarioContext.ScenarioInfo.Tags.FirstOrDefault(t => t.StartsWith("testcase:"));
                int testCaseId = 0;
                if (testCaseTag != null)
                {
                    int.TryParse(testCaseTag.Replace("testcase:", ""), out testCaseId);
                }
                TestContext.Progress.WriteLine($"[BEFORE] Test case ID: {testCaseId}");

                // 2. Set providers
                TestCaseIdProvider.SetTestCaseId(testCaseId);
                TestNameProvider.SetTestName(_featureContext.FeatureInfo.Title);
                TestContext.Progress.WriteLine($"[BEFORE] Providers set");

                // 3. Initialize settings
                _generateExpectedResults = Boolean.Parse(TestContext.Parameters["generateExpectedResults"] ?? "false");
                _testRail = new TestRail();
                TestContext.Progress.WriteLine($"[BEFORE] Settings initialized");
                
                // 4. Create browser
                var slowMo = int.TryParse(TestContext.Parameters["slowMo"], out int slowMoValue) ? slowMoValue : 0;
                var headless = Boolean.Parse(TestContext.Parameters["headless"] ?? "true");
                
                TestContext.Progress.WriteLine($"[BEFORE] Creating Playwright...");
                var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
                
                TestContext.Progress.WriteLine($"[BEFORE] Launching browser (headless: {headless})...");
                _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    SlowMo = headless ? 0 : slowMo
                });
                
                TestContext.Progress.WriteLine($"[BEFORE] Creating browser context...");
                _browserContext = await _browser.NewContextAsync();
                
                TestContext.Progress.WriteLine($"[BEFORE] Creating new page...");
                _page = await _browserContext.NewPageAsync();
                TestContext.Progress.WriteLine($"[BEFORE] Browser ready");

                // 5. Create test objects
                _results = new Results();
                var expectedResultsFolder = TestContext.Parameters["expectedResultsFolder"] ?? "../../../data/expectedResults";
                _expectedResults = new ExpectedResults(_featureContext.FeatureInfo.Title, expectedResultsFolder, _generateExpectedResults);
                _expectedResults.Init();
                TestContext.Progress.WriteLine($"[BEFORE] Test objects created");

                // 6. Share via ScenarioContext
                TestContext.Progress.WriteLine($"[BEFORE] Adding to ScenarioContext...");
                _scenarioContext["Page"] = _page;
                _scenarioContext["ExpectedResults"] = _expectedResults;
                _scenarioContext["Results"] = _results;
                _scenarioContext["TestCaseId"] = testCaseId;
                
                TestContext.Progress.WriteLine($"[BEFORE SCENARIO COMPLETE] {_scenarioContext.ScenarioInfo.Title}");
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

                if (_page != null && !_page.IsClosed) await _page.CloseAsync();
                
                if (_browserContext != null) await _browserContext.CloseAsync();
                
                if (_browser != null) await _browser.CloseAsync();

                _results?.Display();
                string errorMessages = _results?.GetErrorMessages() ?? "";

                if (_results?.HasFailures() == true)
                {
                    _testRail?.AddUnSuccessfulTestRailResult(errorMessages);
                    Assert.Fail($"Scenario Failed: {errorMessages}");
                }
                else
                {
                    _testRail?.AddSuccessfulTestRailResult();
                }

                _expectedResults?.Close();
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
    }
}