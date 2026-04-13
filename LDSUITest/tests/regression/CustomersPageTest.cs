using LDSUITest.data.TestInput;
using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]

    public class CustomersPageTest : BaseTest
    {
        private CustomersPage _customersPage = null!;

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

        [TestCase(2, Description = "Verify filter by customer ID")]
        public async Task VerifyCustomersFilterById(int testCaseId)
        {
            // Get test input data - strongly typed, no parsing needed!
            var testData = CustomersPageTestData.GetTestData(testCaseId);
            string customerId = testData.CustomerId;

            await _customersPage.FilterCustomersById(Page, customerId);
            await BasePage.VerifyPage<CustomersPageData>(Page);
        }

        [TestCase(3, Description = "Verify filter by customer name")]
        public async Task VerifyCustomersFilterByName(int testCaseId)
        {
            // Get test input data - strongly typed, no parsing needed!
            var testData = CustomersPageTestData.GetTestData(testCaseId);
            string customerName = testData.CustomerName;

            await _customersPage.FilterCustomersByName(Page, customerName);
            await BasePage.VerifyPage<CustomersPageData>(Page);
        }
    }
}