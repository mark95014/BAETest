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
    [Parallelizable(ParallelScope.All)] // Allow parallel execution
    public abstract class BaseTest
    {
        // Instance fields - NUnit creates a new instance per test
        public ExpectedResults ExpectedResults = null!;
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

        // Configuration from .runsettings
        protected string _browserType = "chrome";
        protected bool _headless = false;

        [OneTimeSetUp]
        public virtual void BaseSetup()
        {
            TestContext.Progress.WriteLine("BaseSetup");

            GenerateExpectedResults = Boolean.Parse(TestContext.Parameters["generateExpectedResults"] ?? "false");
            _browserType = TestContext.Parameters["browser"] ?? "chrome";
            _headless = Boolean.Parse(TestContext.Parameters["headless"] ?? "false");
            TestName = TestNameProvider.GetTestName();
            TestRail = new TestRail();
            var expectedResultsFolder = TestContext.Parameters["expectedResultsFolder"] ?? "../../../data/expectedResults";
            ExpectedResults = new ExpectedResults(TestName, expectedResultsFolder, GenerateExpectedResults);
            ExpectedResults.Init();
        }

        [OneTimeTearDown]
        public virtual void TestTearDown()
        {
            TestContext.Progress.WriteLine("TestTearDown");
            ExpectedResults?.Close();
        }

        /// <summary>
        /// Creates a WebDriver instance based on browser type
        /// </summary>
        internal IWebDriver CreateWebDriver(string browserType, bool headless)
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

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Manage().Window.Maximize();

            return driver;
        }

        public void TestCaseFinish(Results results)
        {
            results.Display();

            string errorMessages = results.GetErrorMessages();
            try
            {
                Assert.That(results.HasFailures(), Is.False, $"Test {TestContext.CurrentContext.Test.FullName} Failed.");
                TestRail.AddSuccessfulTestRailResult();
            }
            catch (Exception)
            {
                TestRail.AddUnSuccessfulTestRailResult(errorMessages);
                throw new Exception($"Test {TestContext.CurrentContext.Test.FullName} Failed.\n{errorMessages}");
            }
        }
    }
}