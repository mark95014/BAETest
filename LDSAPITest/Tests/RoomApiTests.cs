using FluentAssertions;
using LDSAPITest.Utils;
using LDSTest.Shared;
using NUnit.Framework;
using System.Collections;
using System.Net;
using System.Net.Http.Json;

namespace LDSAPITest.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class RoomApiTests : BaseApiTest
    {
        // Data models
        public class Room
        {
            public int RoomNumber { get; set; }
            public int Price { get; set; }
        }

        // Test data source for GetAllRooms
        private static IEnumerable GetAllRoomsTestCases
        {
            get
            {
                yield return new TestCaseData(20)
                    .SetDescription("Get all rooms");
            }
        }

        // Test data source for GetRoomByNumber
        private static IEnumerable GetRoomByNumberTestCases
        {
            get
            {
                yield return new TestCaseData(21, new Room { RoomNumber = 1001 })
                    .SetDescription("Get room by number 1001");

                yield return new TestCaseData(22, new Room { RoomNumber = 1007 })
                    .SetDescription("Get room by number 1007");
            }
        }

        // Test data source for CreateRoom
        private static IEnumerable CreateRoomTestCases
        {
            get
            {
                yield return new TestCaseData(23, new Room 
                { 
                    RoomNumber = 2001,
                    Price = 150
                })
                .SetDescription("Create new room");
            }
        }

        [Test]
        [TestCaseSource(nameof(GetAllRoomsTestCases))]
        public async Task GetAllRooms_ShouldReturnListOfRooms(int testCaseId)
        {
            var response = await GetAsync("GetAllRooms");

            await EnsureSuccessStatusCodeAsync(response);

            var rooms = await response.Content.ReadFromJsonAsync<List<Room>>();
            rooms.Should().NotBeNull();
            rooms.Should().NotBeEmpty();
            VerifyResponse.Verify(new { rooms = rooms! }, ExpectedResults);

            LogInfo($"Retrieved {rooms?.Count} rooms");
        }

        [TestCaseSource(nameof(GetRoomByNumberTestCases))]
        public async Task GetRoomByNumber_WithValidNumber_ShouldReturnRoom(int testCaseId, Room roomInput)
        {
            int roomNumber = roomInput.RoomNumber;

            var response = await GetAsync($"GetRoom/{roomNumber}");

            await AssertStatusCodeAsync(response, HttpStatusCode.OK);

            var room = await response.Content.ReadFromJsonAsync<Room>();
            VerifyResponse.Verify(new { room = room! }, ExpectedResults);
        }

        [NonParallelizable]
        [TestCaseSource(nameof(CreateRoomTestCases))]
        public async Task CreateRoom_WithValidData_ShouldReturnCreatedRoom(int testCaseId, Room roomData)
        {
            var response = await PostAsync("CreateEditRoom", roomData);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);

            response = await GetAsync("GetAllRooms");
            var rooms = await response.Content.ReadFromJsonAsync<List<Room>>();
            VerifyResponse.Verify(new { rooms = rooms! }, ExpectedResults);

            var room = rooms!.Find(b => b.RoomNumber == roomData.RoomNumber && b.RoomNumber == roomData.RoomNumber);
            response = await DeleteAsync($"DeleteRoom/{room!.RoomNumber}");
            await BaseApiTest.EnsureSuccessStatusCodeAsync(response);
        }
    }
}