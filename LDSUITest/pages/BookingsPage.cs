using LDSTest.Shared;
using LDSUITest.utils.PageData;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace LDSUITest.pages
{
    public class BookingsPage : BasePage
    {
        protected override string RelativePath => "/bookings";

        internal static class Selectors
        {
            internal const string pageTitle = "h1:has-text('All Bookings')";
            internal const string bookingsTable = "[id='bookingsTable']";
            internal const string filterCustomerIdInput = "[id='filterCustomerId']";
            internal const string filterCustomerNameInput = "[id='filterCustomerName']";
            internal const string applyFiltersButton = "button:has-text('Apply Filters')";
            internal const string nextPageButton = "[id='btnNext']";
        }

        // Pre-initialized locators
        public static ILocator PageTitle(IPage page) => page.Locator(Selectors.pageTitle);
        public static ILocator BookingsTable(IPage page) => page.Locator(Selectors.bookingsTable);
        public static ILocator FilterCustomerIdInput(IPage page) => page.Locator(Selectors.filterCustomerIdInput);
        public static ILocator FilterCustomerNameInput(IPage page) => page.Locator(Selectors.filterCustomerNameInput);
        public static ILocator ApplyFiltersButton(IPage page) => page.Locator(Selectors.applyFiltersButton);
        public static ILocator NextPageButton(IPage page) => page.Locator(Selectors.nextPageButton);

        public override async Task WaitForPageToLoad(IPage page)
        {
            await PageTitle(page).WaitForAsync();
            await Expect(PageTitle(page)).ToBeVisibleAsync();
            await BookingsTable(page).WaitForAsync();
            await NextPageButton(page).WaitForAsync();
        }

        public async Task FilterBookingsByCustomerId(IPage page, string customerId)
        {
            await FilterCustomerIdInput(page).FillAsync(customerId);
            await ApplyFiltersButton(page).ClickAsync();
            await WaitForPageToLoad(page);
        }

        public async Task FilterBookingsByCustomerName(IPage page, string customerName)
        {
            await FilterCustomerNameInput(page).FillAsync(customerName);
            await ApplyFiltersButton(page).ClickAsync();
            await WaitForPageToLoad(page);
        }

        public async Task VerifyPage(IPage page, ExpectedResults expectedResults, Results results)
        {
            await BasePage.VerifyPage<BookingsPageData>(page, expectedResults, results);
        }
    }
}