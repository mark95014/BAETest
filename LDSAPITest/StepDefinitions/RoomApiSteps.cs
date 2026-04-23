using LDSAPITest.Utils;
using LDSTest.Shared;
using System.Net.Http.Json;
using TechTalk.SpecFlow;

namespace LDSAPITest.StepDefinitions
{
    [Binding]
    public class RoomApiSteps : BaseApiTest
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly ExpectedResults _expectedResults = null!;

        public class Room
        {
            public int RoomNumber { get; set; }
            public int Price { get; set; }
        }

        public RoomApiSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _expectedResults = _scenarioContext.Get<ExpectedResults>("ExpectedResults");
            BaseOneTimeSetUp();
        }

        [Given(@"I have a new room with the following details:")]
        public void GivenIHaveANewRoomWithTheFollowingDetails(Table table)
        {
            var room = new Room
            {
                RoomNumber = int.Parse(table.Rows[0]["Value"]),
                Price = int.Parse(table.Rows[1]["Value"])
            };
            _scenarioContext.Add("NewRoom", room);
        }

        [When(@"I create a new room")]
        public async Task WhenICreateANewRoom()
        {
            var room = _scenarioContext.Get<Room>("NewRoom");
            _scenarioContext["Response"] = await PostAsync("CreateEditRoom", room);
            await new Database().ResetDatabase();
        }

        [When(@"I send a request to get all rooms")]
        public async Task GetAllRooms()
        {
            _scenarioContext["Response"] = await GetAsync("GetAllRooms");
        }

        [When(@"I send a GET request to get room with number (.*)")]
        public async Task GetRoomByNumber(int roomNumber)
        {
            _scenarioContext["Response"] = await GetAsync($"GetRoom/{roomNumber}");
        }

        [Then(@"the response should contain the expected room")]
        public async Task ThenTheResponseShouldContainASingleRoom()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var room = await response.Content.ReadFromJsonAsync<Room>();
            VerifyResponse.Verify(new { room }, _expectedResults);
        }

        [Then(@"the response should contain the expected rooms")]
        public async Task ThenTheResponseShouldContainAListOfRooms()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var list = await response.Content.ReadFromJsonAsync<List<Room>>();
            VerifyResponse.Verify(new { rooms = list }, _expectedResults);
        }
    }
}