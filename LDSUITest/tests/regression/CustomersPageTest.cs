using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;
using System.Collections;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]

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
        public async Task Setup()
        {
            _customersPage = new CustomersPage();
            await _customersPage.GoTo(Page);
        }

        [TestCaseSource(nameof(VerifyCustomersPageTestCases))]
        public async Task VerifyCustomersPage(int testCaseId)
        {
            await BasePage.VerifyPage<CustomersPageData>(Page, ExpectedResults, Results);
        }

        [TestCaseSource(nameof(VerifyCustomersFilterByIdTestCases))]
        public async Task VerifyCustomersFilterById(int testCaseId, string customerId)
        {
            await _customersPage.FilterCustomersById(Page, customerId);
            await BasePage.VerifyPage<CustomersPageData>(Page, ExpectedResults, Results);
        }

        [TestCaseSource(nameof(VerifyCustomersFilterByNameTestCases))]
        public async Task VerifyCustomersFilterByName(int testCaseId, string customerName)
        {
            await _customersPage.FilterCustomersByName(Page, customerName);
            await BasePage.VerifyPage<CustomersPageData>(Page, ExpectedResults, Results);
        }
    }
}