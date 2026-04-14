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
        public required ExpectedResults ExpectedResults;
        public bool Verbose = Boolean.Parse(TestContext.Parameters["verbose"] ?? "false");
        public Results Results = new();  
        public DBServer DbServer = null!;       
        public bool GenerateExpectedResults;
        private int slowMo = 0;
        private bool headless = true;


        // Provide public read-only properties for external access
        public string databaseBackupFileName = string.Empty;
        public string databaseName = string.Empty;
        public string guid = string.Empty;
        public int defaultTimeoutInSeconds = 300;

        public IPage Page { get; private set; } = null!;
        private IBrowser? _customBrowser;
        private IBrowserContext? _customContext;

        public BaseTest()
        {
            //Page = null!; // Will be properly initialized in TestCaseSetUp
        }

        [OneTimeSetUp]
        public virtual void BaseSetup()
        {
            TestContext.Progress.WriteLine("BaseSetup");

            GenerateExpectedResults = Boolean.Parse(TestContext.Parameters["generateExpectedResults"] ?? "false");
            slowMo = int.TryParse(TestContext.Parameters["slowMo"], out int slowMoValue) ? slowMoValue : 0;          
            headless = Boolean.Parse(TestContext.Parameters["headless"] ?? "true");
            
            var testName = TestContext.CurrentContext.Test.Name;
            var expectedResultsFolder = TestContext.Parameters["expectedResultsFolder"] ?? "../../../data/expectedResults";
            
            ExpectedResults = new ExpectedResults(testName, expectedResultsFolder, GenerateExpectedResults);
            ExpectedResults.Init();
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
            ExpectedResults.Close();
        }

        public void SetTestCaseId(string testCaseId)
        {
            TestContext.CurrentContext.Test.Arguments[0] = testCaseId;
        }

        public void TestCaseFinish()
        {
            int testCaseId = LDSTest.Shared.Context.GetTestCaseId();
            Results.Display();

            string errorMessages = Results.GetErrorMessages();

            try
            {
                Assert.That(Results.HasFailures(), Is.False, $"Test {TestContext.CurrentContext.Test.FullName} Failed.");
                TestRail.AddSuccessfulTestRailResult(testCaseId);
            }
            catch (Exception)
            {
                TestRail.AddUnSuccessfulTestRailResult(testCaseId, errorMessages);
            }
        }
    }
}