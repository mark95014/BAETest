using LDSUITest.pages;
using LDSUITest.src.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    public class CustomersPageTest : BaseTest
    {
        private CustomersPage _customersPage;

        [SetUp]
        public async Task Setup()
        {
            _customersPage = new CustomersPage();
            await _customersPage.GoTo(Page);
        }

        [TestCase(1, Description = "Verify all data on Customers page")]
        public async Task VerifyCustomersPage(int testCaseId)
        {
            await BasePage.VerifyPage<CustomersPageData>(Page);
        }

        [TestCase(2, "7", Description = "Verify filter by customer ID")]
        public async Task VerifyCustomersFilterById(int testCaseId, string customerId)
        {
            await _customersPage.FilterCustomersById(Page, customerId);
            await BasePage.VerifyPage<CustomersPageData>(Page);
        }

        [TestCase(3, "son", Description = "Verify filter by customer name")]
        public async Task VerifyCustomersFilterByName(int testCaseId, string customerName)
        {
            await _customersPage.FilterCustomersByName(Page, customerName);
            await BasePage.VerifyPage<CustomersPageData>(Page);
        }
    }
}