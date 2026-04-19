using LDSTest.Shared;
using LDSUITest.utils;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace LDSUITest.Hooks
{
    [Binding]
    public class UITestHooks : BaseTest
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private int _testCaseId;

        public UITestHooks(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            TestContext.Progress.WriteLine($"Scenario: {_scenarioContext.ScenarioInfo.Title}");

            // Get test case ID from tags BEFORE calling BaseSetup
            var testCaseTag = _scenarioContext.ScenarioInfo.Tags
                .FirstOrDefault(t => t.StartsWith("testcase:"));
            _testCaseId = 0;
            if (testCaseTag != null)
            {
                int.TryParse(testCaseTag.Replace("testcase:", ""), out _testCaseId);
            }

            // Set the test case ID in Context for TestRail integration
            if (_testCaseId > 0)
            {
                LDSTest.Shared.Context.SetTestCaseId(_testCaseId);
            }

            // Call BaseTest setup - this initializes parameters
            BaseSetup();
            
            // Override test name with scenario title for SpecFlow
            var scenarioTitle = $"{_featureContext.FeatureInfo.Title}_{_scenarioContext.ScenarioInfo.Title}";
            
            await TestCaseSetUp();

            // Recreate ExpectedResults with proper scenario name
            var expectedResultsFolder = TestContext.Parameters["expectedResultsFolder"] ?? "../../../data/expectedResults";
            ExpectedResults?.Close();
            ExpectedResults = new ExpectedResults(scenarioTitle, expectedResultsFolder, GenerateExpectedResults);
            ExpectedResults.Init();

            // Store shared objects
            _scenarioContext.Add("Page", Page);
            _scenarioContext.Add("BaseTest", this);
            _scenarioContext.Add("ExpectedResults", ExpectedResults);
            _scenarioContext.Add("Results", Results);
            _scenarioContext.Add("TestCaseId", _testCaseId);
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            // Custom teardown logic that avoids TestCaseFinish() which expects NUnit test arguments
            TestContext.Progress.WriteLine("SpecFlow Scenario TearDown");

            try
            {
                if (Page != null && !Page.IsClosed)
                {
                    await Page.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"Error closing page: {ex.Message}");
            }

            // Handle Results and TestRail manually without calling TestCaseFinish()
            if (Results != null)
            {
                Results.Display();
                string errorMessages = Results.GetErrorMessages();

                try
                {
                    if (Results.HasFailures())
                    {
                        TestRail.AddUnSuccessfulTestRailResult(_testCaseId, errorMessages);
                        Assert.Fail($"Scenario '{_scenarioContext.ScenarioInfo.Title}' Failed: {errorMessages}");
                    }
                    else
                    {
                        TestRail.AddSuccessfulTestRailResult(_testCaseId);
                    }
                }
                catch (Exception ex)
                {
                    TestContext.Progress.WriteLine($"Error in TestRail reporting: {ex.Message}");
                }
            }

            // Close ExpectedResults
            ExpectedResults?.Close();

            // Close browser context and browser (instead of calling TestCaseTearDown which calls TestCaseFinish)
            try
            {
                // Access private fields using reflection if needed, or let them be cleaned up
                // The base TestCaseTearDown handles this, but we can't call it because it calls TestCaseFinish
                
                // Call parent's teardown but catch any errors from TestCaseFinish
                try
                {
                    await base.TestCaseTearDown();
                }
                catch (IndexOutOfRangeException)
                {
                    // Expected - ignore the error from TestCaseFinish trying to access Arguments[0]
                    TestContext.Progress.WriteLine("Ignored IndexOutOfRangeException from TestCaseFinish (expected for SpecFlow tests)");
                }
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"Error in teardown: {ex.Message}");
            }

            TestContext.Progress.WriteLine($"Completed: {_scenarioContext.ScenarioInfo.Title}");
        }
    }
}