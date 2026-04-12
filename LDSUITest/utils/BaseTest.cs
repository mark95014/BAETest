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
        // CA2211: Non-constant fields should not be visible
        // Make static fields private and expose via properties if needed

        private static bool verbose;
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
            // Initialize non-nullable fields to prevent CS8618 warnings
            Page = null!; // Will be properly initialized in TestCaseSetUp
        }

        [OneTimeSetUp]
        public virtual void BaseSetup()
        {
            //Log test start
            TestContext.Progress.WriteLine("BaseSetup");
            results = new();
            verbose = Boolean.Parse(TestContext.Parameters["verbose"] ?? "false");
            environment = TestContext.Parameters["environment"]?.ToString() ?? string.Empty;
            generateExpectedResults = Boolean.Parse(TestContext.Parameters["generateExpectedResults"] ?? "false");
            
            // Read slowMo from .runsettings
            var slowMoParam = TestContext.Parameters["slowMo"];
            if (!string.IsNullOrEmpty(slowMoParam) && int.TryParse(slowMoParam, out int slowMoValue))
            {
                slowMo = slowMoValue;
                TestContext.Progress.WriteLine($"SlowMo enabled: {slowMo}ms");
            }
            
            // Read headless from .runsettings
            var headlessParam = TestContext.Parameters["headless"];
            if (!string.IsNullOrEmpty(headlessParam) && bool.TryParse(headlessParam, out bool headlessValue))
            {
                headless = headlessValue;
                TestContext.Progress.WriteLine($"Headless mode: {headless}");
            }
            
            // Get environment-specific web base URL
            webBaseUrl = TestContext.Parameters[$"{environment}.webBaseURL"] 
                         ?? throw new InvalidOperationException($"Web Base URL not configured for environment: {environment}");
            
            TestContext.Progress.WriteLine($"Web Base URL: {webBaseUrl}");
            
            ExpectedResults.Init(GetType().Name, generateExpectedResults, "regression");
        }

        [SetUp]
        public virtual async Task TestCaseSetUp()
        {
            if (headless)
            {
                slowMo = 0; // Disable slowMo when running in headless mode for faster execution
            }

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
            var testCaseIdArg = TestContext.CurrentContext.Test.Arguments[0]?.ToString();
            if (testCaseIdArg == null || !int.TryParse(testCaseIdArg, out int testCaseId))
            {
                throw new InvalidOperationException("Test case ID argument is missing or invalid");
            }
            return testCaseId;
        }

        public static void SetTestCaseId(string testCaseId)
        {
            TestContext.CurrentContext.Test.Arguments[0] = testCaseId;
        }

        public static void TestCaseFinish()
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