using LDSUITest.utils.PageData;
using Microsoft.Playwright;

namespace LDSUITest.pages
{
    internal static class RoomsPage
    {
        public readonly static string url = "https://localhost:7031/rooms";

        internal static class Selectors
        {
            internal static string pageTitle = "h1:has-text('All Rooms')";
            internal static string roomsTable = "[id='roomsTable']";
        }

        internal static async Task GoTo(IPage page)
        {
            await page.GotoAsync(url);
            await WaitForPageToLoad(page);
        }

        internal static async Task WaitForPageToLoad(IPage page)
        {
            await page.WaitForSelectorAsync(Selectors.pageTitle);
            await page.WaitForSelectorAsync(Selectors.roomsTable);
        }

        internal static async Task VerifyPage(IPage page)
        {
            var pageData = new RoomsPageData();
            pageData.Initialize(page);
            await CommonVerifyPage.Verify(pageData);
        }
    }
}