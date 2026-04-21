using LDSTest.Shared;
using LDSUITest.utils;
using Microsoft.Playwright;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace LDSUITest.Hooks
{
    [Binding]
    public class UITestHooks : BaseTest
    {
        public readonly ScenarioContext _scenarioContext;
        public readonly FeatureContext _featureContext;
        public int _testCaseId;
        public IBrowser? _browser;
        public IBrowserContext? _browserContext;

        public UITestHooks(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        } 

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            try
            {
                TestContext.Progress.WriteLine($"[1] Starting BeforeScenario for: {_scenarioContext.ScenarioInfo.Title}");

                // Get test case ID from tags
                var testCaseTag = _scenarioContext.ScenarioInfo.Tags.FirstOrDefault(t => t.StartsWith("testcase:"));
                _testCaseId = 0;
                if (testCaseTag != null)
                {
                    int.TryParse(testCaseTag.Replace("testcase:", ""), out _testCaseId);
                }
                TestContext.Progress.WriteLine($"[2] Test case ID extracted: {_testCaseId}");

                // Register test case ID with the provider
                TestCaseIdProvider.SetTestCaseId(_testCaseId);

                // Get feature title (this is the test name for SpecFlow)
                var featureTitle = _featureContext.FeatureInfo.Title;
                TestNameProvider.SetTestName(featureTitle);
                TestContext.Progress.WriteLine($"[3] Feature title: {featureTitle}");

                // Call BaseTest setup - this initializes parameters
                TestContext.Progress.WriteLine("[4] Calling BaseSetup...");
                BaseSetup();
                TestContext.Progress.WriteLine("[5] BaseSetup completed");
                
                // For SpecFlow, manually initialize the browser
                TestContext.Progress.WriteLine("[6] Initializing browser for SpecFlow...");
                
                var slowMo = int.TryParse(TestContext.Parameters["slowMo"], out int slowMoValue) ? slowMoValue : 0;
                var headless = Boolean.Parse(TestContext.Parameters["headless"] ?? "true");
                slowMo = headless ? 0 : slowMo;

                var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
                _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    SlowMo = slowMo
                });
                
                _browserContext = await _browser.NewContextAsync();
                Page = await _browserContext.NewPageAsync();
                
                TestContext.Progress.WriteLine("[7] Browser initialized");

                // Initialize Results
                Results = new Results();

                TestContext.Progress.WriteLine("[8] Initializing ExpectedResults...");
                var expectedResultsFolder = TestContext.Parameters["expectedResultsFolder"] ?? "../../../data/expectedResults";
                
                if (ExpectedResults != null)
                {
                    ExpectedResults.Close();
                }
                
                // Use featureTitle (test name) for ExpectedResults file name
                ExpectedResults = new ExpectedResults(featureTitle, expectedResultsFolder, GenerateExpectedResults);
                ExpectedResults.Init();
                TestContext.Progress.WriteLine("[9] ExpectedResults initialized");

                // Store shared objects
                TestContext.Progress.WriteLine("[10] Adding objects to ScenarioContext...");
                _scenarioContext.Add("Page", Page);
                _scenarioContext.Add("BaseTest", this);
                _scenarioContext.Add("ExpectedResults", ExpectedResults);
                _scenarioContext.Add("Results", Results);
                _scenarioContext.Add("TestCaseId", _testCaseId);

                TestContext.Progress.WriteLine("[11] BeforeScenario completed successfully");
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"[ERROR] Exception in BeforeScenario: {ex.GetType().Name}");
                TestContext.Progress.WriteLine($"[ERROR] Message: {ex.Message}");
                TestContext.Progress.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            try
            {
                TestContext.Progress.WriteLine("[TEARDOWN 1] Starting AfterScenario");

                if (Page != null && !Page.IsClosed)
                {
                    TestContext.Progress.WriteLine("[TEARDOWN 2] Closing page...");
                    await Page.CloseAsync();
                }

                // Close browser context and browser
                if (_browserContext != null)
                {
                    await _browserContext.CloseAsync();
                }

                if (_browser != null)
                {
                    await _browser.CloseAsync();
                }

                // Handle Results and TestRail - pass testCaseId explicitly
                if (Results != null)
                {
                    TestContext.Progress.WriteLine("[TEARDOWN 3] Processing results...");
                    Results.Display(); // Pass testCaseId as parameter
                    string errorMessages = Results.GetErrorMessages();

                    try
                    {
                        if (Results.HasFailures())
                        {
                            TestContext.Progress.WriteLine($"[TEARDOWN 4] Test failed: {errorMessages}");
                            TestRail.AddUnSuccessfulTestRailResult(errorMessages);
                            Assert.Fail($"Scenario '{_scenarioContext.ScenarioInfo.Title}' Failed: {errorMessages}");
                        }
                        else
                        {
                            TestContext.Progress.WriteLine("[TEARDOWN 5] Test passed");
                            TestRail.AddSuccessfulTestRailResult();
                        }
                    }
                    catch (Exception ex)
                    {
                        TestContext.Progress.WriteLine($"[TEARDOWN ERROR] TestRail reporting error: {ex.Message}");
                    }
                }

                // Close ExpectedResults
                TestContext.Progress.WriteLine("[TEARDOWN 6] Closing ExpectedResults...");
                ExpectedResults?.Close();

                TestContext.Progress.WriteLine($"[TEARDOWN 7] Completed: {_scenarioContext.ScenarioInfo.Title}");
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"[TEARDOWN FATAL ERROR] {ex.GetType().Name}: {ex.Message}");
                TestContext.Progress.WriteLine($"[TEARDOWN FATAL ERROR] StackTrace: {ex.StackTrace}");
                // Don't rethrow - we're in cleanup
            }
            finally
            {
                // Always clear the test case ID
                TestCaseIdProvider.Clear();
            }
        }
    }
}