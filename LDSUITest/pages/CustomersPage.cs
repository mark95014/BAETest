using LDSUITest.utils.PageData;
using Microsoft.Playwright;

namespace LDSUITest.pages
{
    internal static class CustomersPage
    {
        public readonly static string url = "https://localhost:7031/customers";

        internal static class Selectors
        {
            internal const string pageTitle = "h1:has-text('All Customers')";
            internal const string customersTable = "[id='customersTable']";
            internal const string filterIdInput = "[id='filterId']";
            internal const string filterNameInput = "[id='filterName']";
            internal const string applyFiltersButton = "button:has-text('Apply Filters')";
        }

        // Pre-initialized locators
        private static ILocator PageTitle(IPage page) => page.Locator(Selectors.pageTitle);
        private static ILocator CustomersTable(IPage page) => page.Locator(Selectors.customersTable);
        private static ILocator FilterIdInput(IPage page) => page.Locator(Selectors.filterIdInput);
        private static ILocator FilterNameInput(IPage page) => page.Locator(Selectors.filterNameInput);
        private static ILocator ApplyFiltersButton(IPage page) => page.Locator(Selectors.applyFiltersButton);

        internal static async Task GoTo(IPage page)
        {
            await page.GotoAsync(url);
            await WaitForPageToLoad(page);
        }

        internal static async Task WaitForPageToLoad(IPage page)
        {
            await PageTitle(page).WaitForAsync();
            await CustomersTable(page).WaitForAsync();
        }

        internal static async Task FilterCustomersById(IPage page, string filterValue)
        {
            await FilterIdInput(page).FillAsync(filterValue);
            await ApplyFiltersButton(page).ClickAsync();
            await WaitForPageToLoad(page);
        }

        internal static async Task FilterCustomersByName(IPage page, string filterValue)
        {
            await FilterNameInput(page).FillAsync(filterValue);
            await ApplyFiltersButton(page).ClickAsync();
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