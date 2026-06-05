using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using NUnit.Framework;
using TechTalk.SpecFlow;
using LDSTest.Shared;

namespace LDSUITest.Hooks
{
    [Binding]
    public class UITestHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        private IWebDriver _driver = null!;
        private TestRail _testRail = new();

        public UITestHooks(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        [BeforeFeature]
        public static async Task BeforeFeatureAsync(FeatureContext featureContext)
        {
            await new Database().ResetDatabase();
        }

        [AfterFeature]
        public static async Task AfterFeatureAsync(FeatureContext featureContext)
        {
            await new Database().ResetDatabase();
        }

        [BeforeScenario(Order = 0)]
        public void BeforeScenario()
        {
            try
            {
                InitializeTestMetadata();
                InitializeSeleniumBrowser();
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
        public void AfterScenario()
        {
            try
            {
                CleanupBrowser();

                var results = _scenarioContext.Get<Results>("Results");
                var expectedResults = _scenarioContext.Get<ExpectedResults>("ExpectedResults");

                CaptureFailureScreenshotIfNeeded();

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

        private void InitializeSeleniumBrowser()
        {
            var headless = Boolean.Parse(TestContext.Parameters["headless"] ?? "false");
            var browserType = TestContext.Parameters["browser"] ?? "chrome";

            switch (browserType.ToLower())
            {
                case "chrome":
                    new DriverManager().SetUpDriver(new ChromeConfig());
                    var chromeOptions = new ChromeOptions();
                    if (headless)
                    {
                        chromeOptions.AddArgument("--headless=new");
                    }
                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--disable-dev-shm-usage");
                    chromeOptions.AddArgument("--ignore-certificate-errors");
                    _driver = new ChromeDriver(chromeOptions);
                    break;

                default:
                    throw new ArgumentException($"Unsupported browser: {browserType}");
            }

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            _driver.Manage().Window.Maximize();
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
            _scenarioContext["Driver"] = _driver;
        }

        private void CleanupBrowser()
        {
            try
            {
                _driver.Quit();
                _driver?.Dispose();
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"[CLEANUP ERROR] {ex.Message}");
            }
        }

        private void CaptureFailureScreenshotIfNeeded()
        {
            if (_scenarioContext.TestError != null && _driver != null)
            {
                try
                {
                    var screenshotDriver = (ITakesScreenshot)_driver;
                    var screenshot = screenshotDriver.GetScreenshot();
                    
                    var screenshotPath = Path.Combine("screenshots", $"{_featureContext.FeatureInfo.Title}.png");
                    Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);
                    screenshot.SaveAsFile(screenshotPath);
                }
                catch (Exception ex)
                {
                    TestContext.Progress.WriteLine($"[SCREENSHOT ERROR] {ex.Message}");
                }
            }
        }
    }
}