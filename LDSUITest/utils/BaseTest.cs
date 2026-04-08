using LDSTest.Shared;
using LDSUITest.utils;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using TestContext = NUnit.Framework.TestContext;

namespace LDSUITest.src.utils
{
    [TestFixture]

    public abstract class BaseTest : PageTest
    {
        public static bool verbose;
        public string environment;
        public string databaseBackupFileName;
        public static Results results;
        public DBServer dbServer;
        public string databaseName;
        public static bool generateExpectedResults;
        public string trxUserName;
        public string trxPassword;
        public string guid;
        public int defaultTimeoutInSeconds = 300;


        [OneTimeSetUp]
        public virtual void BaseSetup()
        {
            //Log test start
            TestContext.Progress.WriteLine("BaseSetup");
            results = new Results();
            verbose = Boolean.Parse(TestContext.Parameters["verbose"]);
            environment = TestContext.Parameters["environment"].ToString();
            generateExpectedResults = Boolean.Parse(TestContext.Parameters["generateExpectedResults"]);
            ExpectedResults.Init(GetType().Name, generateExpectedResults, "regression");
        }

        [SetUp]
        public virtual void TestCaseSetUp()
        {

        }

        [TearDown]
        public virtual void TestCaseTearDown()
        {
            //log test case finish
            TestContext.Progress.WriteLine("TestCaseTearDown");
            TestCaseFinish();
        }

        [OneTimeTearDown]
        public virtual void TestTearDown()
        {
            TestContext.Progress.WriteLine("TestTearDown");
            //Test.LogOut();
            //Database.Cleanup(Test.dbServer, Test.guid);
            ExpectedResults.Close(generateExpectedResults);
        }

        public static int GetTestCaseId()
        {
            int testCaseId = int.Parse(TestContext.CurrentContext.Test.Arguments[0].ToString());
            return testCaseId;
        }

        public static void SetTestCaseId(string testCaseId)
        {
            TestContext.CurrentContext.Test.Arguments[0] = testCaseId;
        }

        public void TestCaseFinish()
        {
            int testCaseId = GetTestCaseId();
            results.Display();

            string errorMessages = results.GetErrorMessages();

            try
            {
                NUnit.Framework.Assert.That(results.HasFailures(), NUnit.Framework.Is.False, $"Test {TestContext.CurrentContext.Test.FullName} Failed.");
                TestRail.AddSuccessfulTestRailResult(testCaseId);
            }
            catch (Exception)
            {
                TestRail.AddUnSuccessfulTestRailResult(testCaseId, errorMessages);
            }
        }
    }
}