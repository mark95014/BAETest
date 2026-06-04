using LDSAPITest;
using LDSAPITest.Tests;
using LDSTest.Shared;
using LDSUITest.pages;
using OpenQA.Selenium;
using System.Net;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using static LDSUITest.pages.RoomsPage;

namespace LDSUITest.StepDefinitions
{
    [Binding]
    public class RoomsSteps(ScenarioContext scenarioContext)
    {
        private readonly IWebDriver _driver = scenarioContext.Get<IWebDriver>("Driver");
        private readonly RoomsPage _roomsPage = new();
        private readonly ExpectedResults _expectedResults = scenarioContext.Get<ExpectedResults>("ExpectedResults");
        private readonly Results _results = scenarioContext.Get<Results>("Results");

        [Given(@"I navigate to the rooms page")]
        public void GivenINavigateToTheRoomsPage()
        {
            _roomsPage.GoTo(_driver);
        }

        [Then(@"the rooms table should contain the expected data")]
        public void ThenTheRoomsTableShouldContainTheExpectedData()
        {
            _roomsPage.VerifyPage(_driver, _expectedResults, _results);
            GivenINavigateToTheRoomsPage();
        }

        [When(@"I edit the following rooms:")]
        public void WhenIEditTheFollowingRooms(Table table)
        {
            var rooms = table.CreateSet<Room>().ToList();
            _roomsPage.EditRoomPrices(_driver, rooms);
            GivenINavigateToTheRoomsPage();
        }

        [Then(@"I restore the original prices")]
        public void ThenIRestoreTheOriginalPrices()
        {
            var originalRooms = new List<RoomsPage.Room>
            {
                new() { RoomNumber = 101, Price = 75 },
                new() { RoomNumber = 103, Price = 78 },
                new() { RoomNumber = 105, Price = 85 }
            };

            _roomsPage.EditRoomPrices(_driver, originalRooms); // Revert changes to original values
            GivenINavigateToTheRoomsPage();
        }
    }
}