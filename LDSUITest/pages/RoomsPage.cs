using LDSTest.Shared;
using LDSUITest.utils.PageData;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace LDSUITest.pages
{
    public class RoomsPage : BasePage
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
            internal const string priceInput = "[id='price']";
            internal const string saveButton = ".btn-save";
        }

        // Pre-initialized locators
        private static ILocator PageTitle(IPage page) => page.Locator(Selectors.pageTitle);
        private static ILocator RoomsTable(IPage page) => page.Locator(Selectors.roomsTable);
        private static ILocator NextPageButton(IPage page) => page.Locator(Selectors.nextPageButton);
        private static ILocator PriceInput(IPage page) => page.Locator(Selectors.priceInput);
        private static ILocator SaveButton(IPage page) => page.Locator(Selectors.saveButton);

        public override async Task WaitForPageToLoad(IPage page)
        {
            await PageTitle(page).WaitForAsync();
            await RoomsTable(page).WaitForAsync();
            await NextPageButton(page).WaitForAsync();
            await NextPageButton(page).IsEnabledAsync(); // Ensure the button is enabled, indicating the table is fully loaded and interactive
            Thread.Sleep(500); // Additional wait to ensure all dynamic content is fully loaded and interactive
        }

        public async Task EditRoomPrices(IPage page, List<Room> rooms)
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
                // Clear may execute before the price loads, so we wait for the input to have a value before clearing it
                await Expect(PriceInput(page)).Not.ToHaveValueAsync("");
                await PriceInput(page).ClearAsync();
                await PriceInput(page).FillAsync(newPrice);
                await SaveButton(page).ClickAsync();

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