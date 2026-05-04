using LDSTest.Shared;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using TestContext = NUnit.Framework.TestContext;

namespace LDSUITest.utils
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public abstract class BaseTest : PageTest
    {
        public ExpectedResults ExpectedResults = null!;
        public bool Verbose = Boolean.Parse(TestContext.Parameters["verbose"] ?? "false");
        public Results Results = null!;
        public TestRail TestRail = null!;
        public DBServer DbServer = null!;       
        public bool GenerateExpectedResults;
        public string TestName = null!;

        // Provide public read-only properties for external access
        public string databaseBackupFileName = string.Empty;
        public string databaseName = string.Empty;
        public string guid = string.Empty;
        public int defaultTimeoutInSeconds = 300;

        public BaseTest()
        {
        }

        [OneTimeSetUp]
        public virtual void BaseSetup()
        {
            GenerateExpectedResults = Boolean.Parse(TestContext.Parameters["generateExpectedResults"] ?? "false");
            TestName = TestNameProvider.GetTestName();
            TestRail = new TestRail();
            var expectedResultsFolder = TestContext.Parameters["expectedResultsFolder"] ?? "../../../data/expectedResults";
            ExpectedResults = new ExpectedResults(TestName, expectedResultsFolder, GenerateExpectedResults);
            ExpectedResults.Init();
        }

        [SetUp]
        public virtual async Task TestCaseSetUp()
        {   
            Results = new Results();
            
            await Task.CompletedTask;
        }

        [TearDown]
        public virtual async Task TestCaseTearDown()
        {
            TestCaseFinish();
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