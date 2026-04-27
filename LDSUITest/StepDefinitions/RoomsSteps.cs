using LDSAPITest;
using LDSAPITest.Tests;
using LDSTest.Shared;
using LDSUITest.pages;
using Microsoft.Playwright;
using System.Net;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using static LDSUITest.pages.RoomsPage;

namespace LDSUITest.StepDefinitions
{
    [Binding]
    public class RoomsSteps(ScenarioContext scenarioContext)
    {
        private readonly IPage _page = scenarioContext.Get<IPage>("Page");
        private readonly RoomsPage _roomsPage = new();
        private readonly ExpectedResults _expectedResults = scenarioContext.Get<ExpectedResults>("ExpectedResults");
        private readonly Results _results = scenarioContext.Get<Results>("Results");

        [Given(@"I navigate to the rooms page")]
        public async Task GivenINavigateToTheRoomsPage()
        {
            await _roomsPage.GoTo(_page);
        }

        [Then(@"the rooms table should contain the expected data")]
        public async Task ThenTheRoomsTableShouldContainTheExpectedData()
        {
            await _roomsPage.VerifyPage(_page, _expectedResults, _results);
        }

        [When(@"I edit the following rooms:")]
        public async Task WhenIEditTheFollowingRooms(Table table)
        {
            var rooms = table.CreateSet<Room>().ToList();
            await _roomsPage.EditRoomPrices(_page, rooms, _expectedResults, _results);
        }
    }
}