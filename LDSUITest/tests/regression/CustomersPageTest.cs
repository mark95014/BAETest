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
            await Page.GotoAsync("https://localhost:7031/customers");

            _customersPageData = new CustomersData();
            _customersPageData.Initialize(Page);
        }

        [TestCase(1, Description = "Verify all data on Customers page")]
        public async Task VerifyCustomersPage(int testCaseId)
        {
            await CustomersPage.VerifyPage(Page);
        }

        [TestCase(2, Description = "Verify filter by customer ID")]
        public async Task VerifyCustomersFilterById(int testCaseId)
        {
            await CustomersPage.FilterCustomersById(Page, "7");
            await CustomersPage.VerifyPage(Page);
        }

        [TestCase(3, Description = "Verify filter by customer name")]
        public async Task VerifyCustomersFilterByName(int testCaseId)
        {
            await CustomersPage.FilterCustomersByName(Page, "son");
            await CustomersPage.VerifyPage(Page);
        }
    }
}