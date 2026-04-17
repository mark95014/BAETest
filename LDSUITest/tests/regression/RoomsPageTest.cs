using LDSTest.Shared;
using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;
using System.Collections;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    // This test modifies the database and should not be run in parallel with other tests.
    // This will be solved when I implement each test having its own database copy.
    [Parallelizable(ParallelScope.None)]

    public class RoomsPageTest : BaseTest
    {
        private RoomsPage _roomsPage = null!;

        // Test data model
        public class Room
        {
            public int RoomNumber { get; set; }
            public int Price { get; set; }
        }

        // Test data source for VerifyRoomsPage
        private static IEnumerable VerifyRoomsPageTestCases
        {
            get
            {
                yield return new TestCaseData(1)
                    .SetDescription("Verify all data on Rooms page");
            }
        }

        // Test data source for VerifyEditRoomFunctionality
        private static IEnumerable VerifyEditRoomFunctionalityTestCases
        {
            get
            {
                yield return new TestCaseData(2, new List<Room>
                {
                    new() { RoomNumber = 101, Price = 150 },
                    new() { RoomNumber = 103, Price = 200 },
                    new() { RoomNumber = 105, Price = 175 }
                })
                .SetDescription("Verify edit room functionality");
            }
        }

        [SetUp]
        public async Task Setup()
        {
            _roomsPage = new RoomsPage();
            await _roomsPage.GoTo(Page);
        }

        [TestCaseSource(nameof(VerifyRoomsPageTestCases))]
        public async Task VerifyRoomsPage(int testCaseId)
        {
            await BasePage.VerifyPage<RoomsPageData>(Page, ExpectedResults, Results);
        }

        [NonParallelizable]
        [TestCaseSource(nameof(VerifyEditRoomFunctionalityTestCases))]
        public async Task VerifyEditRoomFunctionality(int testCaseId, List<Room> rooms)
        {
            foreach (var room in rooms)
            {
                // Use the properties directly
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