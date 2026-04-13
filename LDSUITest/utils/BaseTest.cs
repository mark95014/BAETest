using LDSTest.Shared;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using TestContext = NUnit.Framework.TestContext;

namespace LDSUITest.utils
{
    [TestFixture]
    public abstract class BaseTest : ContextTest
    {
        private static bool verbose = false;
        private static Results results = null!;  
        public DBServer dbServer = null!;       
        private static bool generateExpectedResults;
        private static int slowMo = 0;
        private static bool headless = true;
        private static string webBaseUrl = string.Empty;

        // Provide public read-only properties for external access
        public static bool Verbose => verbose;
        public static Results Results => results;
        public static bool GenerateExpectedResults => generateExpectedResults;
        public static int SlowMo => slowMo;
        public static bool Headless => headless;
        public static string WebBaseUrl => webBaseUrl;

        public string environment = string.Empty;
        public string databaseBackupFileName = string.Empty;
        public string databaseName = string.Empty;
        public string trxUserName = string.Empty;
        public string trxPassword = string.Empty;
        public string guid = string.Empty;
        public int defaultTimeoutInSeconds = 300;

        public IPage Page { get; private set; } = null!;
        private IBrowser? _customBrowser;
        private IBrowserContext? _customContext;

        public BaseTest()
        {
            Page = null!; // Will be properly initialized in TestCaseSetUp
        }

        [OneTimeSetUp]
        public virtual void BaseSetup()
        {
            TestContext.Progress.WriteLine("BaseSetup");
            results = new();
            verbose = Boolean.Parse(TestContext.Parameters["verbose"] ?? "false");
            environment = TestContext.Parameters["environment"]?.ToString() ?? string.Empty;
            generateExpectedResults = Boolean.Parse(TestContext.Parameters["generateExpectedResults"] ?? "false");
            slowMo = int.TryParse(TestContext.Parameters["slowMo"], out int slowMoValue) ? slowMoValue : 0;          
            headless = Boolean.Parse(TestContext.Parameters["headless"] ?? "true");
            webBaseUrl = TestContext.Parameters[$"{environment}.webBaseURL"] ?? "";

            ExpectedResults.Init(GetType().Name, generateExpectedResults, "regression");
        }

        [SetUp]
        public virtual async Task TestCaseSetUp()
        {
            slowMo = headless ? 0 : slowMo; // Disable slowMo when running in headless mode for faster execution

            // This is the reason for using ContextTest as the base class instead of PageTest
            // to allow for custom browser and context creation with specific options
            _customBrowser = await Context.Browser!.BrowserType.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless,
                SlowMo = slowMo
            });
            
            _customContext = await _customBrowser.NewContextAsync();
            Page = await _customContext.NewPageAsync();
        }

        [TearDown]
        public virtual async Task TestCaseTearDown()
        {
            TestContext.Progress.WriteLine("TestCaseTearDown");
            
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

        //public static int GetTestCaseId()
        //{
        //    // NUnit stores the arguments to a [TestCase] in TestContext. Arg[0] is test case id
        //    int testCaseId = int.Parse(TestContext.CurrentContext.Test.Arguments[0]!.ToString()!);
        //    return testCaseId;
        //}

        public static void SetTestCaseId(string testCaseId)
        {
            TestContext.CurrentContext.Test.Arguments[0] = testCaseId;
        }

        public static void TestCaseFinish()
        {
            int testCaseId = LDSTest.Shared.Context.GetTestCaseId();
            results.Display();

            string errorMessages = results.GetErrorMessages();

            try
            {
                Assert.That(results.HasFailures(), Is.False, $"Test {TestContext.CurrentContext.Test.FullName} Failed.");
                TestRail.AddSuccessfulTestRailResult(testCaseId);
            }
            catch (Exception)
            {
                TestRail.AddUnSuccessfulTestRailResult(testCaseId, errorMessages);
            }
        }
    }
}