using FluentAssertions;
using LDSTest.Shared;
using LDSUITest.pages;
using LDSUITest.utils.PageData;
using Microsoft.Playwright;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace LDSUITest.StepDefinitions
{
    [Binding]
    public class RoomsSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private IPage _page = null!;
        private RoomsPage _roomsPage = null!;
        private ExpectedResults _expectedResults = null!;
        private Results _results = null!;

        public class Room
        {
            public int RoomNumber { get; set; }
            public int Price { get; set; }
        }

        public RoomsSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _page = scenarioContext.Get<IPage>("Page");
            _roomsPage = new RoomsPage();
            _expectedResults = scenarioContext.Get<ExpectedResults>("ExpectedResults");
            _results = scenarioContext.Get<Results>("Results");
        }

        [Given(@"I navigate to the rooms page")]
        public async Task GivenINavigateToTheRoomsPage()
        {
            await _roomsPage.GoTo(_page);
        }

        [Then(@"the rooms table should contain the expected data")]
        public async Task ThenTheRoomsTableShouldContainTheExpectedData()
        {
            await BasePage.VerifyPage<RoomsPageData>(_page, _expectedResults, _results);
        }

        [When(@"I select room number ""(.*)""")]
        public async Task WhenISelectRoomNumber(string roomNumber)
        {
            var row = _page.Locator($"[id='roomsTable'] tbody tr:has(td:nth-child(1):text-is('{roomNumber}'))");
            await row.ClickAsync();
            _scenarioContext["SelectedRoomNumber"] = roomNumber;
        }

        [When(@"I update the room price to ""(.*)""")]
        public async Task WhenIUpdateTheRoomPriceTo(string newPrice)
        {
            await _page.Locator("[id='price']").FillAsync(newPrice);
            _scenarioContext["UpdatedPrice"] = newPrice;
        }

        [When(@"I save the changes")]
        public async Task WhenISaveTheChanges()
        {
            await _page.Locator(".btn-save").ClickAsync();
            await _roomsPage.WaitForPageToLoad(_page);
        }

        [When(@"I edit the following rooms:")]
        public async Task WhenIEditTheFollowingRooms(Table table)
        {
            var rooms = table.CreateSet<Room>();

            foreach (var room in rooms)
            {
                string roomNumber = room.RoomNumber.ToString();
                string newPrice = room.Price.ToString();

                // Find and click the row
                var row = _page.Locator($"[id='roomsTable'] tbody tr:has(td:nth-child(1):text-is('{roomNumber}'))");
                await row.ClickAsync();

                // Edit the price
                await _page.Locator("[id='price']").FillAsync(newPrice);
                await _page.Locator(".btn-save").ClickAsync();

                // Wait for the table to reload
                await _roomsPage.WaitForPageToLoad(_page);
            }

            _scenarioContext["EditedRooms"] = rooms.ToList();
        }

        [Then(@"the room price should be updated to ""(.*)"" for room ""(.*)""")]
        public async Task ThenTheRoomPriceShouldBeUpdatedToForRoom(string expectedPrice, string roomNumber)
        {
            // Find the room row
            var row = _page.Locator($"[id='roomsTable'] tbody tr:has(td:nth-child(1):text-is('{roomNumber}'))");

            // Get the price cell (adjust column index as needed - assuming price is in column 2)
            var priceCell = row.Locator("td:nth-child(2)");
            var actualPrice = await priceCell.TextContentAsync();

            actualPrice.Should().Be(expectedPrice, $"Room {roomNumber} should have price {expectedPrice}");
        }

        [Then(@"the rooms table should reflect the updated prices")]
        public async Task ThenTheRoomsTableShouldReflectTheUpdatedPrices()
        {
            var testCaseId = _scenarioContext.Get<int>("TestCaseId");
            await BasePage.VerifyPage<RoomsPageData>(_page, _expectedResults, _results);
        }

        [Then(@"the database should be reset for test isolation")]
        public async Task ThenTheDatabaseShouldBeResetForTestIsolation()
        {
            await new Database().ResetDatabase();
        }

        [Given(@"the rooms table has multiple pages")]
        public async Task GivenTheRoomsTableHasMultiplePages()
        {
            var nextButton = _page.Locator("[id='btnNext']");
            var isEnabled = await nextButton.IsEnabledAsync();
            isEnabled.Should().BeTrue("Next page button should be enabled, indicating multiple pages exist");
        }

        [When(@"I click the next page button")]
        public async Task WhenIClickTheNextPageButton()
        {
            await _page.Locator("[id='btnNext']").ClickAsync();
            await _roomsPage.WaitForPageToLoad(_page);
        }

        [Then(@"the next page of rooms should be displayed")]
        public async Task ThenTheNextPageOfRoomsShouldBeDisplayed()
        {
            var roomsTable = _page.Locator("[id='roomsTable']");
            (await roomsTable.IsVisibleAsync()).Should().BeTrue("Rooms table should be visible on the next page");
        }

        [Then(@"the rooms table should contain valid room data")]
        public async Task ThenTheRoomsTableShouldContainValidRoomData()
        {
            var rows = await _page.Locator("[id='roomsTable'] tbody tr").CountAsync();
            rows.Should().BeGreaterThan(0, "Rooms table should contain at least one row of data");
        }

        [When(@"I refresh the page")]
        public async Task WhenIRefreshThePage()
        {
            await _page.ReloadAsync();
            await _roomsPage.WaitForPageToLoad(_page);
        }

        [Then(@"the room price should still be ""(.*)"" for room ""(.*)""")]
        public async Task ThenTheRoomPriceShouldStillBeForRoom(string expectedPrice, string roomNumber)
        {
            await ThenTheRoomPriceShouldBeUpdatedToForRoom(expectedPrice, roomNumber);
        }
    }
}