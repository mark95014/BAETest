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
        private TestRail _testRail = new();

        public ApiTestHooks(ScenarioContext scenarioContext, FeatureContext featureContext)
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
        public void BeforeScenario()
        {
            try
            {
                InitializeTestMetadata();

                InitializeResultsAndExpectedResults();
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

                var results = _scenarioContext.Get<Results>("Results");
                var expectedResults = _scenarioContext.Get<ExpectedResults>("ExpectedResults");

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
    }
}