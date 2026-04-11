using LDSTest.Shared;
using LDSUITest.pages;
using LDSUITest.src.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    public class RoomsPageTest : BaseTest
    {
        private RoomsPage _roomsPage;

        [SetUp]
        public async Task Setup()
        {
            _roomsPage = new RoomsPage();
            await _roomsPage.GoTo(Page);
        }

        [TestCase(1, Description = "Verify all data on Rooms page")]
        public async Task VerifyRoomsPage(int testCaseId)
        {
            await BasePage.VerifyPage<RoomsPageData>(Page);
        }

        [TestCase(2, Description = "Verify edit room functionality")]
        public async Task VerifyEditRoomFunctionality(int testCaseId)
        {
            var roomsToEdit = new[]
            {
                new { RowIndex = 1, NewPrice = "150" },
                new { RowIndex = 3, NewPrice = "200" },
                new { RowIndex = 5, NewPrice = "175" }
            };

            foreach (var room in roomsToEdit)
            {
                var row = Page.Locator($"[id='roomsTable'] tbody tr:nth-child({room.RowIndex})");
                await row.ClickAsync();

                await Page.Locator("[id='price']").FillAsync(room.NewPrice);
                await Page.Locator(".btn-save").ClickAsync();

                await _roomsPage.WaitForPageToLoad(Page);
            }

            await BasePage.VerifyPage<RoomsPageData>(Page);

            await new Database().ResetDatabase();
        }
    }
}