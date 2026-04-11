using LDSUITest.src.utils;
using LDSUITest.utils.PageData;
using Microsoft.Playwright;

namespace LDSUITest.pages
{
    internal class BookingsPage : BasePage
    {
        protected override string RelativePath => "/bookings";

        internal static class Selectors
        {
            internal const string pageTitle = "h1:has-text('All Bookings')";
            internal const string bookingsTable = "[id='bookingsTable']";
            internal const string filterCustomerIdInput = "[id='filterCustomerId']";
            internal const string filterCustomerNameInput = "[id='filterCustomerName']";
            internal const string applyFiltersButton = "button:has-text('Apply Filters')";
        }

        // Pre-initialized locators
        private static ILocator PageTitle(IPage page) => page.Locator(Selectors.pageTitle);
        private static ILocator BookingsTable(IPage page) => page.Locator(Selectors.bookingsTable);
        private static ILocator FilterCustomerIdInput(IPage page) => page.Locator(Selectors.filterCustomerIdInput);
        private static ILocator FilterCustomerNameInput(IPage page) => page.Locator(Selectors.filterCustomerNameInput);
        private static ILocator ApplyFiltersButton(IPage page) => page.Locator(Selectors.applyFiltersButton);

        public override async Task WaitForPageToLoad(IPage page)
        {
            await PageTitle(page).WaitForAsync();
            await BookingsTable(page).WaitForAsync();
        }

        public async Task FilterBookings(IPage page, string filterValue)
        {
            await FilterCustomerIdInput(page).FillAsync(filterValue);
            await ApplyFiltersButton(page).ClickAsync();
            await WaitForPageToLoad(page);
        }

        public async Task FilterBookingsByCustomerName(IPage page, string filterValue)
        {
            await FilterCustomerNameInput(page).FillAsync(filterValue);
            await ApplyFiltersButton(page).ClickAsync();
            await WaitForPageToLoad(page);
        }
    }
}