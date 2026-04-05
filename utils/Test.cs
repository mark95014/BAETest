using TestContext = NUnit.Framework.TestContext;
using Assert = NUnit.Framework.Assert;
using BAETest.utils;

//Note: I wanted this class to be inherited by each test, but Nunit does not instantiate the class which contains the TestFixture until after the
//OneTimeSetup finishes. So, the startup aspects of the Test class would not be available to the child class when we need them in OneTimeSetup.
//Therefore, I had to make the Test class static.

namespace BAETest.src.utils
{
    public static class Test
    {
        public readonly static bool verbose;
        public readonly static string environment;
        public readonly static string databaseBackupFileName;
        //public readonly static IWebDriver driver;
        public readonly static Results results;
        public readonly static string webURL;
        public readonly static DBServer dbServer;
        public readonly static string databaseName;
        public readonly static bool generateExpectedResults;
        public readonly static string trxUserName;
        public readonly static string trxPassword;
        public readonly static string guid;
        public readonly static int defaultTimeoutInSeconds = 300;
        public readonly static bool delayRebalance = true;

        //guid is used for both database name and user name. guid must be 9 chars, max. 'T' is prepended when the Trx user name is formed.
        //User name has max 10 chars.

        public class TestCaseIdentifier
        {
            public int testCaseId;
            public string fqdn;

            public TestCaseIdentifier()
            {
                this.testCaseId = int.Parse(TestContext.CurrentContext.Test.Arguments[0].ToString());
                this.fqdn = $"{TestContext.CurrentContext.Test.FullName}";
            }
        }

        static Test()
        {
            Test.results = new Results();
            Test.verbose = Boolean.Parse(TestContext.Parameters["verbose"]);
            Test.environment = TestContext.Parameters["environment"].ToString();

            Test.dbServer = new DBServer(TestContext.Parameters[$"{Test.environment}.databaseServer"].ToString(),
                                         TestContext.Parameters["databaseUserName"].ToString(), 
                                         TestContext.Parameters["databasePassword"].ToString());

            if (Test.environment == "prod")
            {
                Test.trxUserName = "TrxAutomation";
                Test.trxPassword = "Password4you";
            }
            else
            {
                Test.guid = Guid.NewGuid().ToString().Substring(0, 8);
                Test.trxUserName = 'T' + guid;
                Test.trxPassword = TestContext.Parameters["trxPassword"].ToString();
                Test.databaseName = TestContext.Parameters["databaseRootName"] + "_" + guid;
            }

            if (Test.environment == "local")
            {
                Test.webURL = TestContext.Parameters["localWebURL"].ToString();
            }
            else if (Test.environment == "prod")
            {
                Test.webURL = "https://trx.morningstar.com";
            }
            else
            {
                Test.webURL = TestContext.Parameters["webURL"].ToString();
                Test.webURL = Test.webURL.Replace("environment", Test.environment);
            }

            Test.generateExpectedResults = Boolean.Parse(TestContext.Parameters["generateExpectedResults"]);
            Test.delayRebalance = Boolean.Parse(TestContext.Parameters["delayRebalance"]);
        }

        public static void Startup(string testName, string databaseBackupFileName = "default.bak", string folder = "regression")
        {
            //TestRail.InitTestRail();
            ExpectedResults.Init(testName, Test.generateExpectedResults, folder);
            //Database.Restore(Test.dbServer, Test.guid, databaseBackupFileName);   Activate this when we get UAT running again.
            //Test.LogIn();
            //HomePage.WaitForPageToLoad();
            //Menu.InitializeMenu();
        }

        public static void LogIn()
        {
            //LoginPage.LogIn(Test.webURL, Test.trxUserName, Test.trxPassword);
        }

        public static void LogOut()
        {
            //var signOutLink = Test.driver.FindElement(By.CssSelector("#signOut"));
            //Thread.Sleep(1000);
            //signOutLink.Click();
        }

        public static int GetTestCaseId()
        {
            int testCaseId = int.Parse(TestContext.CurrentContext.Test.Arguments[0].ToString());
            return testCaseId;
        }

        public static void SetTestCaseId(string  testCaseId)
        {
            TestContext.CurrentContext.Test.Arguments[0] = testCaseId;
        }

        public static void TestCaseFinish()
        {
            int testCaseId = Test.GetTestCaseId();
            results.Display();

            string errorMessages = Test.results.GetErrorMessages();

            try
            {
                Assert.That(Test.results.HasFailures(), NUnit.Framework.Is.False, $"Test {TestContext.CurrentContext.Test.FullName} Failed.");
                TestRail.AddSuccessfulTestRailResult(testCaseId);
            }
            catch (Exception)
            {
                TestRail.AddUnSuccessfulTestRailResult(testCaseId, errorMessages);
            }
        }

        public static void Finish()
        {
            Test.LogOut();
            Database.Cleanup(Test.dbServer, Test.guid);
            ExpectedResults.Close(Test.generateExpectedResults);
        }
    }
}