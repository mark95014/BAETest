using LDSAPITest.Utils;
using LDSTest.Shared;
using System.Net;
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
            _scenarioContext.Add("Room", room);
        }

        [When(@"I create a new room")]
        public async Task WhenICreateANewRoom()
        {
            var room = _scenarioContext.Get<Room>("Room");
            var response = await PostAsync("CreateEditRoom", room);
            await AssertStatusCodeAsync(response, HttpStatusCode.OK);
            room = await response.Content.ReadFromJsonAsync<Room>();
            await BaseApiTest.EnsureSuccessStatusCodeAsync(response);
            _scenarioContext["Room"] = room;
        }

        [Then(@"I delete the room just created")]
        public async Task DeleteTheRoomJustCreated()
        {
            var room = _scenarioContext.Get<Room>("Room");
            var response = await DeleteAsync($"DeleteRoom/{room!.RoomNumber}");
            await BaseApiTest.EnsureSuccessStatusCodeAsync(response);
        }

        [When(@"I send a request to get all rooms")]
        public async Task GetAllRooms()
        {
            var response = await GetAsync("GetAllRooms");
            await BaseApiTest.AssertStatusCodeAsync(response, HttpStatusCode.OK);
            _scenarioContext["Response"] = response;

            var list = await response.Content.ReadFromJsonAsync<List<Room>>();
            _scenarioContext["Rooms"] = list;
        }

        [When(@"I send a GET request to get room with number (.*)")]
        public async Task GetRoomByNumber(int roomNumber)
        {
            var response = await GetAsync($"GetRoom/{roomNumber}");
            await AssertStatusCodeAsync(response, HttpStatusCode.OK);
            _scenarioContext["Response"] = response;
            _scenarioContext["Room"] = await response.Content.ReadFromJsonAsync<Room>();
        }

        [Then(@"the response should contain the expected room")]
        public void ThenTheResponseShouldContainASingleRoom()
        {
            var room = _scenarioContext.Get<Room>("Room");
            VerifyResponse.Verify(new { room }, _expectedResults);
        }

        [Then(@"the response should contain the expected rooms")]
        public void VerifyTheListOfRooms()
        {
            var list = _scenarioContext.Get<List<Room>>("Rooms");
            VerifyResponse.Verify(new { rooms = list }, _expectedResults);
        }

        [Then(@"the response should contain the original list of rooms plus the new room")]
        public void VerifyModifiedListOfRooms()
        {
            VerifyTheListOfRooms();
        }
    }
}