using LDSUITest.pages;
using LDSUITest.src.utils;
using NUnit.Framework;
using CustomersData = LDSUITest.utils.PageData.CustomersPageData;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    public class CustomersPageTest : BaseTest
    {
        private CustomersData _customersPageData;

        [SetUp]
        public async Task Setup()
        {
            await CustomersPage.GoTo(Page);

            _customersPageData = new CustomersData();
            _customersPageData.Initialize(Page);
        }

        [TestCase(1, Description = "Verify all data on Customers page")]
        public async Task VerifyCustomersPage(int testCaseId)
        {
            await CustomersPage.VerifyPage(Page);
        }

        [TestCase(2, "7", Description = "Verify filter by customer ID")]
        public async Task VerifyCustomersFilterById(int testCaseId, string customerId)
        {
            await CustomersPage.FilterCustomersById(Page, customerId);
            await CustomersPage.VerifyPage(Page);
        }

        [TestCase(3, "son", Description = "Verify filter by customer name")]
        public async Task VerifyCustomersFilterByName(int testCaseId, string customerName)
        {
            await CustomersPage.FilterCustomersByName(Page, customerName);
            await CustomersPage.VerifyPage(Page);
        }
    }
}