namespace LDSAPITest.data.TestInput
{
    public static class BookingApiTestData
    {
        // Dictionary to look up test data by testCaseId
        private static readonly Dictionary<int, BookingApiTestCase> _testCases = new()
        {
            [2] = new BookingApiTestCase
            {
                Booking = new BookingInput
                {
                    Id = 1
                }
            },
            [5] = new BookingApiTestCase
            {
                Booking = new BookingInput
                {
                    Id = 7
                }
            },
            [3] = new BookingApiTestCase
            {
                Booking = new BookingInput
                {
                    Id = 0,
                    CustomerId = 7,
                    RoomNumber = 1007
                }
            }
        };

        public static BookingApiTestCase GetTestData(int testCaseId)
        {
            if (_testCases.TryGetValue(testCaseId, out var testCase))
            {
                return testCase;
            }
            throw new KeyNotFoundException($"No test data found for testCaseId: {testCaseId}");
        }
    }

    public class BookingApiTestCase
    {
        public BookingInput Booking { get; set; } = new();
    }

    public class BookingInput
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int RoomNumber { get; set; }
    }
}