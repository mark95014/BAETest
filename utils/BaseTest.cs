using NUnit.Framework;
using TestContext = NUnit.Framework.TestContext;

namespace BAETest.src.utils
{
    [TestFixture]

    public abstract class BaseTest
    {
        [OneTimeSetUp]
        public virtual void BaseSetup()
        {
            //Log test start
            Test.Startup(GetType().Name);
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
            Test.TestCaseFinish();
        }

        [OneTimeTearDown]
        public virtual void TestTearDown()
        {
            //log test finish
            TestContext.Progress.WriteLine("TestTearDown");
            Test.Finish();
        }
    }
}