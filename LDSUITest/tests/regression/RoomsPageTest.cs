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
        private static IEnumerable EditRoomTestCases
        {
            get
            {
                yield return new TestCaseData(2, new List<RoomsPage.Room>
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
        [TestCaseSource(nameof(EditRoomTestCases))]
        public async Task EditRoom(int testCaseId, List<RoomsPage.Room> rooms)
        {
            RoomsPage roomsPage = new RoomsPage();
            await roomsPage.EditRoomPrices(Page, rooms);

            // Verify the entire table after all edits
            await BasePage.VerifyPage<RoomsPageData>(Page, ExpectedResults, Results);

            await roomsPage.GoTo(Page);

            var originalRooms = new List<RoomsPage.Room>
                {
                    new() { RoomNumber = 101, Price = 75 },
                    new() { RoomNumber = 103, Price = 78 },
                    new() { RoomNumber = 105, Price = 85 }
                };

            await roomsPage.EditRoomPrices(Page, originalRooms); // Revert changes to original values
        }
    }
}