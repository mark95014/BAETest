using Azure;
using LDSTest.Shared;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using Microsoft.Playwright;

namespace LDSUITest.pages
{
    internal class RoomsPage : BasePage
    {
        protected override string RelativePath => "/rooms";

        public class Room
        {
            public int RoomNumber { get; set; }
            public int Price { get; set; }
        }

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
            await NextPageButton(page).IsEnabledAsync(); // Ensure the button is enabled, indicating the table is fully loaded and interactive
            Thread.Sleep(500); // Additional wait to ensure all dynamic content is fully loaded and interactive
        }

        public async Task EditRoomPrices(IPage page, List<Room> rooms, ExpectedResults expectedResults, Results results)
        {
            foreach (var room in rooms)
            {
                // Use the properties directly
                string roomNumber = room.RoomNumber.ToString();
                string newPrice = room.Price.ToString();

                // Find the row with the matching room number in the specific column
                var row = page.Locator($"[id='roomsTable'] tbody tr:has(td:nth-child(1):text-is('{roomNumber}'))");
                await row.ClickAsync();

                // Edit the price
                await page.Locator("[id='price']").FillAsync(newPrice);
                await page.Locator(".btn-save").ClickAsync();

                // Wait for the table to reload
                await WaitForPageToLoad(page);
            }
        }

        public async Task VerifyPage(IPage page, ExpectedResults expectedResults, Results results)
        {
            await BasePage.VerifyPage<RoomsPageData>(page, expectedResults, results);
        }
    }
}