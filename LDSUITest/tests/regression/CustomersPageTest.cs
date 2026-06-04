using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;
using System.Collections;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class CustomersPageTest : BaseTest
    {
        private CustomersPage _customersPage = null!;

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
            _customersPage = new CustomersPage();
            _customersPage.GoTo(Driver);  // Changed: Page → Driver, removed await
        }

        [TestCaseSource(nameof(VerifyCustomersPageTestCases))]
        public void VerifyCustomersPage(int testCaseId)  // Changed: removed async Task
        {
            BasePage.VerifyPage<CustomersPageData>(Driver, ExpectedResults, Results);  // Changed: Page → Driver, removed await
        }

        [TestCaseSource(nameof(VerifyCustomersFilterByIdTestCases))]
        public void VerifyCustomersFilterById(int testCaseId, string customerId)  // Changed: removed async Task
        {
            _customersPage.FilterCustomersById(Driver, customerId);  // Changed: Page → Driver, removed await
            BasePage.VerifyPage<CustomersPageData>(Driver, ExpectedResults, Results);  // Changed: Page → Driver, removed await
        }

        [TestCaseSource(nameof(VerifyCustomersFilterByNameTestCases))]
        public void VerifyCustomersFilterByName(int testCaseId, string customerName)  // Changed: removed async Task
        {
            _customersPage.FilterCustomersByName(Driver, customerName);  // Changed: Page → Driver, removed await
            BasePage.VerifyPage<CustomersPageData>(Driver, ExpectedResults, Results);  // Changed: Page → Driver, removed await
        }
    }
}