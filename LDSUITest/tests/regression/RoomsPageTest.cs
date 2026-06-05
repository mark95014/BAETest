using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)] // Allow test methods to run in parallel
    public class RoomsPageTest : BaseTest
    {
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
        public void Setup()
        {
        }

        [Test]
        [TestCaseSource(nameof(VerifyRoomsPageTestCases))]
        public void VerifyRoomsPage(int testCaseId)
        {
            IWebDriver driver = CreateWebDriver(_browserType, _headless);
            new RoomsPage().GoTo(driver);
            BasePage.VerifyPage<RoomsPageData>(driver, ExpectedResults, Results);
            driver.Quit();
        }

        [Test]
        [NonParallelizable] // This test must run alone because it modifies the database
        [TestCaseSource(nameof(EditRoomTestCases))]
        public void EditRoom(int testCaseId, List<RoomsPage.Room> rooms)
        {
            IWebDriver driver = CreateWebDriver(_browserType, _headless);
            RoomsPage roomsPage = new RoomsPage();
            roomsPage.GoTo(driver);
            roomsPage.EditRoomPrices(driver, rooms);

            // Verify the entire table after all edits
            BasePage.VerifyPage<RoomsPageData>(driver, ExpectedResults, Results);

            roomsPage.GoTo(driver);

            var originalRooms = new List<RoomsPage.Room>
            {
                new() { RoomNumber = 101, Price = 75 },
                new() { RoomNumber = 103, Price = 78 },
                new() { RoomNumber = 105, Price = 85 }
            };

            roomsPage.EditRoomPrices(driver, originalRooms); // Revert changes to original values
            
            driver.Quit();
        }
    }
}