using LDSTest.Shared;
using LDSUITest.data.TestInput;
using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;


namespace LDSUITest.tests.regression
{
    [TestFixture]
    // This test modifies the database and should not be run in parallel with other tests.
    // This will be solved when I implement each test having its own database copy.
    [Parallelizable(ParallelScope.None)]

    public class RoomsPageTest : BaseTest
    {
        private RoomsPage _roomsPage = null!;

        [SetUp]
        public async Task Setup()
        {
            _roomsPage = new RoomsPage();
            await _roomsPage.GoTo(Page);
        }

        [TestCase(1, Description = "Verify all data on Rooms page")]
        public async Task VerifyRoomsPage(int testCaseId)
        {
            await BasePage.VerifyPage<RoomsPageData>(Page, ExpectedResults, Results);
        }

        [NonParallelizable]
        [TestCase(2, Description = "Verify edit room functionality")]
        //[Ignore("This test modifies the database and should not be run in parallel with other tests. Consider refactoring to ensure test isolation.")]
        public async Task VerifyEditRoomFunctionality(int testCaseId)
        {
            // Get test input data - strongly typed, no parsing needed!
            var testData = RoomsPageTestData.GetTestData(testCaseId);

            foreach (var room in testData.Rooms)
            {
                // Use the DTO properties directly
                string roomNumber = room.RoomNumber.ToString();
                string newPrice = room.Price.ToString();

                // Find the row with the matching room number in the specific column
                var row = Page.Locator($"[id='roomsTable'] tbody tr:has(td:nth-child(1):text-is('{roomNumber}'))");
                await row.ClickAsync();

                // Edit the price
                await Page.Locator("[id='price']").FillAsync(newPrice);
                await Page.Locator(".btn-save").ClickAsync();

                // Wait for the table to reload
                await _roomsPage.WaitForPageToLoad(Page);
            }

            // Verify the entire table after all edits
            await BasePage.VerifyPage<RoomsPageData>(Page, ExpectedResults, Results);

            // Reset the database to ensure test isolation
            await new Database().ResetDatabase();
        }
    }
}