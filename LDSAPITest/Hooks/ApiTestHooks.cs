using LDSTest.Shared;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace LDSAPITest.Hooks
{
    [Binding]
    public class ApiTestHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        public ApiTestHooks(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        [BeforeFeature]
        public static async Task BeforeFeature(FeatureContext featureContext)
        {
            await new Database().ResetDatabase();
            var expectedResultsFolder = TestContext.Parameters["expectedResultsFolder"] ?? "../../../data/expectedResults";
            var generateExpectedResults = Boolean.Parse(TestContext.Parameters["generateExpectedResults"] ?? "false");

            var results = new Results();
            TestNameProvider.SetTestName(featureContext.FeatureInfo.Title);
            var expectedResults = new ExpectedResults(TestNameProvider.GetTestName(), expectedResultsFolder, generateExpectedResults);
            expectedResults.Init();

            // Store the once-per-feature instances in the FeatureContext so they can be shared across scenarios
            featureContext["TestRail"] = new TestRail();
            featureContext["Results"] = results;
            featureContext["ExpectedResults"] = expectedResults;
        }

        [AfterFeature]
        public static async Task AfterFeature(FeatureContext featureContext)
        {
            await new Database().ResetDatabase();
            featureContext["TestRail"] = null;
            featureContext["Results"] = null;
            var expectedResults = featureContext["ExpectedResults"] as ExpectedResults;
            expectedResults!.Close();
        }

        [BeforeScenario(Order = 0)]
        public void BeforeScenario()
        {
            try
            {
                InitializeTestCaseId();

                // Save the shared objects from FeatureContext to ScenarioContext for easier access in step definitions
                _scenarioContext["Results"] = _featureContext["Results"];
                _scenarioContext["ExpectedResults"] = _featureContext["ExpectedResults"];
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"[BEFORE SCENARIO ERROR] {ex.GetType().Name}: {ex.Message}");
                TestContext.Progress.WriteLine($"[BEFORE SCENARIO ERROR] Stack: {ex.StackTrace}");
                throw;
            }
        }

        [AfterScenario(Order = 100)]
        public void AfterScenario(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            try
            {

                var results = scenarioContext.Get<Results>("Results");
                var expectedResults = scenarioContext.Get<ExpectedResults>("ExpectedResults");
                var testRail = featureContext.Get<TestRail>("TestRail");

                results.Display();
                string errorMessages = results.GetErrorMessages() ?? "";

                if (results.HasFailures())
                {
                    testRail?.AddUnSuccessfulTestRailResult(errorMessages);
                    Assert.Fail($"Scenario Failed: {errorMessages}");
                }
                else
                {
                    testRail?.AddSuccessfulTestRailResult();
                }
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

        private void InitializeTestCaseId()
        {
            TestCaseIdProvider.SetTestCaseId(ExtractTestCaseId());
        }

        private void SaveScenarioObjects()
        {
            _scenarioContext["Results"] = _featureContext["Results"];
            _scenarioContext["ExpectedResults"] = _featureContext["ExpectedResults"];
        }
    }
}