using BAETest.src.utils;
using BAETest.utils;
using BAETest.src.utils.PageData;
using Microsoft.Playwright;
using BAETest.utils.PageData;

namespace BAETest.pages
{
    internal static class BookingsPage
    {
        public readonly static string url = "https://localhost:7031/bookings";

        internal static class Selectors
        {
            internal static string pageTitle = "h1:has-text('All Bookings')";
            internal static string bookingsTable = "[id='bookingsTable']";
            internal static string filterCustomerIdDropdown = "[id='filterCustomerId']";
        }

        internal static async Task GoTo(IPage page)
        {
            await page.GotoAsync(url);
            await WaitForPageToLoad(page);
        }

        internal static async Task WaitForPageToLoad(IPage page)
        {
            await page.WaitForSelectorAsync(Selectors.pageTitle);
            await page.WaitForSelectorAsync(Selectors.bookingsTable);
        }

        internal static async Task FilterBookings(IPage page, string filterValue)
        {
            await page.ClickAsync(Selectors.filterCustomerIdDropdown);
            await page.ClickAsync($"text={filterValue}");
        }

        internal static void VerifyPage()
        {
            CommonVerifyPage.Verify(new BookingsPageData());
        }
    }
}