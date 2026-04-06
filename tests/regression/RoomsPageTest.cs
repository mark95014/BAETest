using LDSTest.pages;
using LDSTest.src.utils;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using RoomsData = LDSTest.utils.PageData.RoomsPageData;

namespace LDSTest.tests.regression
{
    [TestFixture]
    public class RoomsPageTest : BaseTest
    {
        private RoomsData _roomsPageData;

        [SetUp]
        public async Task Setup()
        {
            await Page.GotoAsync("https://localhost:7031/rooms");

            _roomsPageData = new RoomsData();
            _roomsPageData.Initialize(Page);
        }

        [TestCase(1, Description = "Verify all data on Rooms page")]
        public async Task VerifyRoomsPage(int testCaseId)
        {
            await RoomsPage.VerifyPage(Page);
        }

        [TestCase(2, Description = "Verify edit room functionality")]
        public async Task VerifyEditRoomFunctionality(int testCaseId)
        {
            // Define the rooms to edit (row indices) and new prices
            // But, we will cancel the changes after editing to ensure the test is non-destructive
            var roomsToEdit = new[]
            {
                new { RowIndex = 1, NewPrice = "150" },
                new { RowIndex = 3, NewPrice = "200" },
                new { RowIndex = 5, NewPrice = "175" }
            };

            foreach (var room in roomsToEdit)
            {
                // Click on the room row to open edit dialog
                var row = Page.Locator($"[id='roomsTable'] tbody tr:nth-child({room.RowIndex})");
                await row.ClickAsync();

                // Wait for the edit dialog/modal to appear
                await Page.WaitForSelectorAsync("[id='price']");

                // Change the price
                await Page.Locator("[id='price']").FillAsync(room.NewPrice);

                // Save the changes
                await Page.Locator(".btn-cancel").ClickAsync();

                // Wait for the dialog to close and page to update
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }

            // Verify the entire table using the standard verification
            await RoomsPage.VerifyPage(Page);
        }
    }
}