using LDSUITest.utils.PageData;
using Microsoft.Playwright;

namespace LDSUITest.pages
{
    internal static class RoomsPage
    {
        public readonly static string url = "https://localhost:7031/rooms";

        internal static class Selectors
        {
            internal const string pageTitle = "h1:has-text('All Rooms')";
            internal const string roomsTable = "[id='roomsTable']";
        }

        // Pre-initialized locators
        private static ILocator PageTitle(IPage page) => page.Locator(Selectors.pageTitle);
        private static ILocator RoomsTable(IPage page) => page.Locator(Selectors.roomsTable);

        internal static async Task GoTo(IPage page)
        {
            await page.GotoAsync(url);
            await WaitForPageToLoad(page);
        }

        internal static async Task WaitForPageToLoad(IPage page)
        {
            await PageTitle(page).WaitForAsync();
            await RoomsTable(page).WaitForAsync();
        }

        internal static async Task VerifyPage(IPage page)
        {
            var pageData = new RoomsPageData();
            pageData.Initialize(page);
            await CommonVerifyPage.Verify(pageData);
        }
    }
}