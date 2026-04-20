using Gurock.TestRail;
using LDSTest.Shared;
using Newtonsoft.Json.Linq;
using TestContext = NUnit.Framework.TestContext;



namespace LDSUITest.utils
{
    public class TestRail
    {
        private bool enableTestRail;
        private readonly int testRailFailedStatus = 5;
        private readonly int testRailPassedStatus = 1;
        private readonly APIClient testRailClient = new("https://ssa.testrail.com/");
        private int testRailRunId;
        private readonly bool debug = false;
        private readonly int testCaseId;

        public TestRail()
        {
            testCaseId = TestCaseIdProvider.GetTestCaseId();
        }

        public string AddSuccessfulTestRailResult(string msg = "")
        {
            if (enableTestRail) return AddTestRailResult(testCaseId, testRailPassedStatus, $"Test passed {msg}");
            else return "";
        }

        public string AddUnSuccessfulTestRailResult(int testCaseId, string msg)
        {
            if (enableTestRail) return AddTestRailResult(testCaseId, testRailFailedStatus, msg);
            else return "";
        }

        private string AddTestRailResult(int caseId, int status, string message)
        {
            if (enableTestRail)
            {
                var data = new Dictionary<string, object> { { "status_id", status }, { "comment", message } };
                string endPoint = $"add_result_for_case/{testRailRunId}/{caseId}";
                _ = (JObject)testRailClient.SendPost(endPoint, data);
            }
            return message;
        }

        private int AddTestRailRun(string name, int milestoneId, int projectId)
        {
            if (enableTestRail)
            {
                var data = new Dictionary<string, object> { { "milestone_id", milestoneId }, { "name", name } };
                string endPoint = $"add_run/{projectId}";
                JObject response = (JObject)testRailClient.SendPost(endPoint, data);
                return (int)response["id"]!;
            }
            else return 0;
        }

        public int UpdateTestRailRun(string name)
        {
            if (enableTestRail)
            {
                var data = new Dictionary<string, object> { { "name", name } };
                string endPoint = $"update_run/{testRailRunId}";
                JObject response = (JObject)testRailClient.SendPost(endPoint, data);
                return (int)response["id"]!;
            }
            else return 0;
        }

        public void InitTestRail(string appVersion)
        {
            if (debug) TestContext.Progress.WriteLine($"NUnit.Framework.TestContext {NUnit.Framework.TestContext.Parameters}");

            //environment variables take precedence

            string enableTestRailVar = Environment.GetEnvironmentVariable("ENABLE_TEST_RAIL")!;
            string runsettingsEnableTestRail = TestContext.Parameters["enableTestRail"]!.ToString().Trim().ToLower();
            try { enableTestRail = bool.Parse(enableTestRailVar ?? runsettingsEnableTestRail); }
            catch (Exception) { enableTestRail = false; }

            string testRailRunIdVar = Environment.GetEnvironmentVariable("TEST_RAIL_RUN_ID")!;
            string runsettingstestRailRunId = TestContext.Parameters["testRailRunId"]!.ToString().Trim();
            string runIdStr = testRailRunIdVar ?? runsettingstestRailRunId;

            if (enableTestRail)
            {
                testRailClient.User = "MSAutomationTest@morningstar.com";
                testRailClient.Password = "msauto";

                int projectId = int.Parse(TestContext.Parameters["projectId"]!.ToString().ToLower().Trim());
                int milestoneId = int.Parse(TestContext.Parameters["milestoneId"]!.ToString().ToLower().Trim());
                string deployment = TestContext.Parameters["environment"]!.ToString();

                string runName = $"Automated test: {deployment} environment, branch {appVersion}";

                if (runIdStr.Length > 0)
                {
                    testRailRunId = int.Parse(runIdStr); //Use existing run
                }
                else
                {
                    testRailRunId = AddTestRailRun(runName, milestoneId, projectId); //Start a new run
                }
            }
        }
    }
}