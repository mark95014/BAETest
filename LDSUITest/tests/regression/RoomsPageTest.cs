using LDSTest.Shared;
using LDSUITest.pages;
using LDSUITest.src.utils;
using Microsoft.Playwright;
using NUnit.Framework;
using RoomsData = LDSUITest.utils.PageData.RoomsPageData;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    public class RoomsPageTest : BaseTest
    {
        private RoomsData _roomsPageData;

        [SetUp]
        public async Task Setup()
        {
            await RoomsPage.GoTo(Page);

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

                await Page.Locator("[id='price']").FillAsync(room.NewPrice);
                await Page.Locator(".btn-save").ClickAsync();

                // Wait for the table to reload after save
                await RoomsPage.WaitForPageToLoad(Page);
            }

            // Verify the entire table using the standard verification
            await RoomsPage.VerifyPage(Page);

            // Reset the database to ensure test isolation and non-destructive testing
            await new Database().ResetDatabase();
        }
    }
}