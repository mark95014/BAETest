using LDSTest.Shared;
using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public class RoomsPageTest : BaseTest
    {
        // Test data sources
        private static IEnumerable VerifyRoomsPageTestCases
        {
            get { yield return new TestCaseData(1).SetDescription("Verify all data on Rooms page"); }
        }

        [Test]
        [TestCaseSource(nameof(VerifyRoomsPageTestCases))]
        public void VerifyRoomsPage(int testCaseId)
        {
            Results results = new Results();
            IWebDriver driver = CreateWebDriver(_browserType, _headless);
            new RoomsPage().GoTo(driver);
            BasePage.VerifyPage<RoomsPageData>(driver, ExpectedResults, results);
            TestCaseFinish(results);
            driver.Quit();
        }
    }
}