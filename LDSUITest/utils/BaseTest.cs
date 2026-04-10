using LDSTest.Shared;
using LDSUITest.utils;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using TestContext = NUnit.Framework.TestContext;

namespace LDSUITest.src.utils
{
    [TestFixture]
    public abstract class BaseTest : ContextTest
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
        public static int slowMo = 0;
        
        public IPage Page { get; private set; } = null!;
        private IBrowser? _customBrowser;
        private IBrowserContext? _customContext;

        [OneTimeSetUp]
        public virtual void BaseSetup()
        {
            //Log test start
            TestContext.Progress.WriteLine("BaseSetup");
            results = new Results();
            verbose = Boolean.Parse(TestContext.Parameters["verbose"]);
            environment = TestContext.Parameters["environment"].ToString();
            generateExpectedResults = Boolean.Parse(TestContext.Parameters["generateExpectedResults"]);
            
            // Read slowMo from .runsettings
            var slowMoParam = TestContext.Parameters["slowMo"];
            if (!string.IsNullOrEmpty(slowMoParam) && int.TryParse(slowMoParam, out int slowMoValue))
            {
                slowMo = slowMoValue;
                TestContext.Progress.WriteLine($"SlowMo enabled: {slowMo}ms");
            }
            
            ExpectedResults.Init(GetType().Name, generateExpectedResults, "regression");
        }

        [SetUp]
        public virtual async Task TestCaseSetUp()
        {
            // Create a new browser with SlowMo if needed
            if (slowMo > 0)
            {
                _customBrowser = await Context.Browser!.BrowserType.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false,  // Show browser when SlowMo is enabled
                    SlowMo = slowMo
                });
                
                _customContext = await _customBrowser.NewContextAsync();
                Page = await _customContext.NewPageAsync();
            }
            else
            {
                // Use default context when no SlowMo
                Page = await Context.NewPageAsync();
            }
        }

        [TearDown]
        public virtual async Task TestCaseTearDown()
        {
            //log test case finish
            TestContext.Progress.WriteLine("TestCaseTearDown");
            
            // Clean up in reverse order of creation
            if (Page != null)
            {
                await Page.CloseAsync();
            }
            
            if (_customContext != null)
            {
                await _customContext.CloseAsync();
                _customContext = null;
            }
            
            if (_customBrowser != null)
            {
                await _customBrowser.CloseAsync();
                _customBrowser = null;
            }
            
            TestCaseFinish();
        }

        [OneTimeTearDown]
        public virtual void TestTearDown()
        {
            TestContext.Progress.WriteLine("TestTearDown");
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