using static LDSTest.Shared.DTO;

namespace LDSUITest.data.TestInput
{
    public static class RoomsPageTestData
    {
        // Dictionary to look up test data by testCaseId
        private static readonly Dictionary<int, RoomTestCase> _testCases = new()
        {
            [2] = new RoomTestCase
            {
                Rooms =
                [
                    new Room { RoomNumber = 101, Price = 150 },
                    new Room { RoomNumber = 103, Price = 200 },
                    new Room { RoomNumber = 105, Price = 175 }
                ]
            },
            // Add more test cases as needed
            [3] = new RoomTestCase
            {
                Rooms =
                [
                    new Room { RoomNumber = 102, Price = 125 }
                ]
            }
        };

        public static RoomTestCase GetTestData(int testCaseId)
        {
            if (_testCases.TryGetValue(testCaseId, out var testCase))
            {
                return testCase;
            }
            throw new KeyNotFoundException($"No test data found for testCaseId: {testCaseId}");
        }
    }

    public class RoomTestCase
    {
        public List<Room> Rooms { get; set; } = [];
    }
}