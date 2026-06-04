using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using LDSTest.Shared;
using TestContext = NUnit.Framework.TestContext;

namespace LDSUITest.utils
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public abstract class BaseTest
    {
        // Test framework objects
        public ExpectedResults ExpectedResults = null!;
        public Results Results = null!;
        public TestRail TestRail = null!;
        public DBServer DbServer = null!;
        
        // Configuration
        public bool GenerateExpectedResults;
        public bool Verbose = Boolean.Parse(TestContext.Parameters["verbose"] ?? "false");
        public string TestName = null!;
        public int defaultTimeoutInSeconds = 300;

        // Database-related properties
        public string databaseBackupFileName = string.Empty;
        public string databaseName = string.Empty;
        public string guid = string.Empty;

        public IWebDriver Driver { get; private set; } = null!;

        // Configuration from .runsettings
        private string _browserType = "chrome";
        private bool _headless = false;

        [OneTimeSetUp]
        public virtual void BaseSetup()
        {
            TestContext.Progress.WriteLine("BaseSetup");

            GenerateExpectedResults = Boolean.Parse(TestContext.Parameters["generateExpectedResults"] ?? "false");
            _browserType = TestContext.Parameters["browser"] ?? "chrome";
            _headless = Boolean.Parse(TestContext.Parameters["headless"] ?? "false");
            TestName = TestNameProvider.GetTestName();
            //TestRail = new TestRail();
        }

        [SetUp]
        public virtual void TestCaseSetUp()
        {
            Results = new Results();

            var expectedResultsFolder = TestContext.Parameters["expectedResultsFolder"] ?? "../../../data/expectedResults";
            ExpectedResults = new ExpectedResults(TestName, expectedResultsFolder, GenerateExpectedResults);
            ExpectedResults.Init();

            // Create WebDriver based on browser type
            //IWebDriver Driver = new ChromeDriver();
            Driver = CreateWebDriver(_browserType, _headless);
            
            // Set implicit wait
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            
            // Maximize window
            Driver.Manage().Window.Maximize();
        }

        [TearDown]
        public virtual void TestCaseTearDown()
        {
            TestContext.Progress.WriteLine("TestCaseTearDown");

            // Close browser
            try
            {
                Driver?.Quit();
                Driver?.Dispose();
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"Error closing driver: {ex.Message}");
            }

            // Complete test results
            TestCaseFinish();

            // Close ExpectedResults
            ExpectedResults?.Close();

            Thread.Sleep(5000);
        }

        [OneTimeTearDown]
        public virtual void TestTearDown()
        {
            TestContext.Progress.WriteLine("TestTearDown");
        }

        /// <summary>
        /// Creates a WebDriver instance based on browser type
        /// </summary>
        private IWebDriver CreateWebDriver(string browserType, bool headless)
        {
            IWebDriver driver;

            switch (browserType.ToLower())
            {
                case "chrome":
                    var chromeOptions = new ChromeOptions();
                    string userProfile = Environment.GetEnvironmentVariable("USERPROFILE") ?? string.Empty;

                    chromeOptions.AddUserProfilePreference("download.default_directory", $"{userProfile}\\Downloads");
                    chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                    chromeOptions.AddUserProfilePreference("download.directory_upgrade", true);
                    chromeOptions.AddUserProfilePreference("safebrowsing.enabled", true);

                    if (headless)
                    {
                        chromeOptions.AddArgument("--headless=new");
                    }
                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--disable-dev-shm-usage");
                    chromeOptions.AddArgument("--ignore-certificate-errors");
                    driver = new ChromeDriver(chromeOptions);
                    break;

                case "firefox":
                    new DriverManager().SetUpDriver(new FirefoxConfig());
                    var firefoxOptions = new FirefoxOptions();
                    if (headless)
                    {
                        firefoxOptions.AddArgument("--headless");
                    }
                    driver = new FirefoxDriver(firefoxOptions);
                    break;

                case "edge":
                    new DriverManager().SetUpDriver(new EdgeConfig());
                    var edgeOptions = new EdgeOptions();
                    if (headless)
                    {
                        edgeOptions.AddArgument("--headless=new");
                    }
                    driver = new EdgeDriver(edgeOptions);
                    break;

                default:
                    throw new ArgumentException($"Unsupported browser type: {browserType}");
            }

            return driver;
        }

        public void TestCaseFinish()
        {
            Results.Display();
            string errorMessages = Results.GetErrorMessages();

            try
            {
                Assert.That(Results.HasFailures(), Is.False, 
                    $"Test {TestContext.CurrentContext.Test.FullName} Failed.");
                //TestRail.AddSuccessfulTestRailResult();
            }
            catch (Exception)
            {
                TestRail.AddUnSuccessfulTestRailResult(errorMessages);
            }
        }
    }
}