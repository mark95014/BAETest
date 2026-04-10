using LDSUITest.utils.PageData;
using Microsoft.Playwright;

namespace LDSUITest.pages
{
    internal static class CustomersPage
    {
        public readonly static string url = "https://localhost:7031/customers";

        internal static class Selectors
        {
            internal static string pageTitle = "h1:has-text('All Customers')";
            internal static string customersTable = "[id='customersTable']";
            internal static string filterIdInput = "[id='filterId']";
            internal static string filterNameInput = "[id='filterName']";
            internal static string applyFiltersButton = "button:has-text('Apply Filters')";
        }

        internal static async Task GoTo(IPage page)
        {
            await page.GotoAsync(url);
            await WaitForPageToLoad(page);
        }

        internal static async Task WaitForPageToLoad(IPage page)
        {
            await page.WaitForSelectorAsync(Selectors.pageTitle);
            await page.WaitForSelectorAsync(Selectors.customersTable);
        }

        internal static async Task FilterCustomersById(IPage page, string filterValue)
        {
            await page.Locator(Selectors.filterIdInput).FillAsync(filterValue);
            await page.ClickAsync(Selectors.applyFiltersButton);
            await WaitForPageToLoad(page);
        }

        internal static async Task FilterCustomersByName(IPage page, string filterValue)
        {
            await page.Locator(Selectors.filterNameInput).FillAsync(filterValue);
            await page.ClickAsync(Selectors.applyFiltersButton);
            await WaitForPageToLoad(page);
        }

        internal static async Task VerifyPage(IPage page)
        {
            var pageData = new CustomersPageData();
            pageData.Initialize(page);
            await CommonVerifyPage.Verify(pageData);
        }
    }
}