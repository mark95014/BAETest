using Azure;
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
            internal const string nextPageButton = "[id='btnNext']";
        }

        // Pre-initialized locators
        private static ILocator PageTitle(IPage page) => page.Locator(Selectors.pageTitle);
        private static ILocator RoomsTable(IPage page) => page.Locator(Selectors.roomsTable);
        private static ILocator NextPageButton(IPage page) => page.Locator(Selectors.nextPageButton);

        public override async Task WaitForPageToLoad(IPage page)
        {
            await PageTitle(page).WaitForAsync();
            await RoomsTable(page).WaitForAsync();
            await NextPageButton(page).WaitForAsync();
            Thread.Sleep(1500); // Small delay to ensure any dynamic content has time to load
            await NextPageButton(page).IsEnabledAsync(); // Ensure the button is enabled, indicating the table is fully loaded and interactive
        }
    }
}