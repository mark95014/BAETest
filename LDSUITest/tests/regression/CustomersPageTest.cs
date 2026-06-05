using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)] // Changed: Enable parallel execution of test methods
    public class CustomersPageTest : BaseTest
    {
        // Test data sources
        private static IEnumerable VerifyCustomersPageTestCases
        {
            get { yield return new TestCaseData(1).SetDescription("Verify all data on Customers page"); }
        }

        private static IEnumerable VerifyCustomersFilterByIdTestCases
        {
            get { yield return new TestCaseData(2, "7").SetDescription("Verify filter by customer ID"); }
        }

        private static IEnumerable VerifyCustomersFilterByNameTestCases
        {
            get { yield return new TestCaseData(3, "son").SetDescription("Verify filter by customer name"); }
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCaseSource(nameof(VerifyCustomersPageTestCases))]
        public void VerifyCustomersPage(int testCaseId)
        {
            IWebDriver driver = CreateWebDriver(_browserType, _headless);
            new CustomersPage().GoTo(driver);
            BasePage.VerifyPage<CustomersPageData>(driver, ExpectedResults, Results);
            driver.Quit();
        }

        [Test]
        [TestCaseSource(nameof(VerifyCustomersFilterByIdTestCases))]
        public void VerifyCustomersFilterById(int testCaseId, string customerId)
        {
            IWebDriver driver = CreateWebDriver(_browserType, _headless);
            var customersPage = new CustomersPage();
            customersPage.GoTo(driver);
            customersPage.FilterCustomersById(driver, customerId);
            BasePage.VerifyPage<CustomersPageData>(driver, ExpectedResults, Results);
            driver.Quit();
        }

        [Test]
        [TestCaseSource(nameof(VerifyCustomersFilterByNameTestCases))]
        public void VerifyCustomersFilterByName(int testCaseId, string customerName)
        {
            IWebDriver driver = CreateWebDriver(_browserType, _headless);
            var customersPage = new CustomersPage();
            customersPage.GoTo(driver);
            customersPage.FilterCustomersByName(driver, customerName);
            BasePage.VerifyPage<CustomersPageData>(driver, ExpectedResults, Results);
            driver.Quit();
        }
    }
}