using LDSUITest.utils;
using LDSUITest.utils.PageData;
using Microsoft.Playwright;

namespace LDSUITest.pages
{
    internal class RoomsPage : BasePage
    {
        protected override string RelativePath => "/rooms";

        internal static class Selectors
        {
            internal const string pageTitle = "h1:has-text('All Rooms')";
            internal const string roomsTable = "[id='roomsTable']";
        }

        // Pre-initialized locators
        private static ILocator PageTitle(IPage page) => page.Locator(Selectors.pageTitle);
        private static ILocator RoomsTable(IPage page) => page.Locator(Selectors.roomsTable);

        public override async Task WaitForPageToLoad(IPage page)
        {
            await PageTitle(page).WaitForAsync();
            await RoomsTable(page).WaitForAsync();
        }
    }
}