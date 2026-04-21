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
        public ExpectedResults ExpectedResults = null!;
        public bool Verbose = Boolean.Parse(TestContext.Parameters["verbose"] ?? "false");
        public Results Results = null!;
        public TestRail TestRail = null!;
        public DBServer DbServer = null!;       
        public bool GenerateExpectedResults;
        private int slowMo = 0;
        private bool headless = true;
        public string TestName = null!;

        // Provide public read-only properties for external access
        public string databaseBackupFileName = string.Empty;
        public string databaseName = string.Empty;
        public string guid = string.Empty;
        public int defaultTimeoutInSeconds = 300;

        public IPage Page { get; protected set; } = null!;
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
            TestName = TestNameProvider.GetTestName();
            TestRail = new TestRail();
        }

        [SetUp]
        public virtual async Task TestCaseSetUp()
        {   
            Results = new Results();

            // Create ExpectedResults per test, not per fixture
            var expectedResultsFolder = TestContext.Parameters["expectedResultsFolder"] ?? "../../../data/expectedResults";
            ExpectedResults = new ExpectedResults(TestName, expectedResultsFolder, GenerateExpectedResults);
            ExpectedResults.Init();
            
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

            try
            {
                if (_customContext != null)
                {
                    await _customContext.CloseAsync();
                    _customContext = null;
                }
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"Error closing context: {ex.Message}");
            }

            try
            {
                if (_customBrowser != null)
                {
                    await _customBrowser.CloseAsync();
                    _customBrowser = null;
                }
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"Error closing browser: {ex.Message}");
            }

            // Use the captured TestCaseId
            TestCaseFinish();
            
            // Close ExpectedResults after each test
            ExpectedResults?.Close();

            await Task.CompletedTask;
        }

        [OneTimeTearDown]
        public virtual void TestTearDown()
        {
            TestContext.Progress.WriteLine("TestTearDown");
        }

        public void TestCaseFinish()
        {
            Results.Display();

            string errorMessages = Results.GetErrorMessages();

            try
            {
                Assert.That(Results.HasFailures(), Is.False, $"Test {TestContext.CurrentContext.Test.FullName} Failed.");
                TestRail.AddSuccessfulTestRailResult();
            }
            catch (Exception)
            {
                TestRail.AddUnSuccessfulTestRailResult(errorMessages);
            }
        }
    }
}