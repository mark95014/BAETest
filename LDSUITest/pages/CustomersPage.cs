using LDSTest.Shared;
using LDSUITest.utils.PageData;
using Microsoft.Playwright;

namespace LDSUITest.pages
{
    internal class CustomersPage : BasePage
    {
        protected override string RelativePath => "/customers";

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

        public override async Task WaitForPageToLoad(IPage page)
        {
            await PageTitle(page).WaitForAsync();
            await CustomersTable(page).WaitForAsync();
        }

        public async Task FilterCustomersById(IPage page, string filterValue)
        {
            await FilterIdInput(page).FillAsync(filterValue);
            await ApplyFiltersButton(page).ClickAsync();
            await WaitForPageToLoad(page);
        }

        public async Task FilterCustomersByName(IPage page, string filterValue)
        {
            await FilterNameInput(page).FillAsync(filterValue);
            await ApplyFiltersButton(page).ClickAsync();
            await WaitForPageToLoad(page);
        }

        public async Task VerifyPage(IPage page, ExpectedResults expectedResults, Results results)
        {
            await BasePage.VerifyPage<CustomersPageData>(page, expectedResults, results);
        }
    }
}