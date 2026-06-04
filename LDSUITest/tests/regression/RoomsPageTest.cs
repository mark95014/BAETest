using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;
using System.Collections;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)] // EditRoom() modifies the database, so it should not be run in parallel with other tests cases or tests.

    public class RoomsPageTest : BaseTest
    {
        private RoomsPage _roomsPage = null!;

        // Test data sources
        private static IEnumerable VerifyRoomsPageTestCases
        {
            get { yield return new TestCaseData(1).SetDescription("Verify all data on Rooms page"); }
        }

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
        public void Setup()  // Changed: removed async Task
        {
            _roomsPage = new RoomsPage();
            _roomsPage.GoTo(Driver);  // Changed: Page → Driver, removed await
        }

        [TestCaseSource(nameof(VerifyRoomsPageTestCases))]
        public void VerifyRoomsPage(int testCaseId)  // Changed: removed async Task
        {
            BasePage.VerifyPage<RoomsPageData>(Driver, ExpectedResults, Results);  // Changed: Page → Driver, removed await
        }

        [NonParallelizable]
        [TestCaseSource(nameof(EditRoomTestCases))]
        public void EditRoom(int testCaseId, List<RoomsPage.Room> rooms)  // Changed: removed async Task
        {
            RoomsPage roomsPage = new RoomsPage();
            roomsPage.EditRoomPrices(Driver, rooms);  // Changed: Page → Driver, removed await

            // Verify the entire table after all edits
            BasePage.VerifyPage<RoomsPageData>(Driver, ExpectedResults, Results);  // Changed: Page → Driver, removed await

            roomsPage.GoTo(Driver);  // Changed: Page → Driver, removed await

            var originalRooms = new List<RoomsPage.Room>
            {
                new() { RoomNumber = 101, Price = 75 },
                new() { RoomNumber = 103, Price = 78 },
                new() { RoomNumber = 105, Price = 85 }
            };

            roomsPage.EditRoomPrices(Driver, originalRooms); // Changed: Page → Driver, removed await - Revert changes to original values
        }
    }
}