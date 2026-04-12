namespace LDSUITest.data.TestInput
{
    public static class BookingsPageTestData
    {
        // Dictionary to look up test data by testCaseId
        private static readonly Dictionary<int, BookingTestCase> _testCases = new()
        {
            [2] = new BookingTestCase
            {
                CustomerId = "7"
            },
            [3] = new BookingTestCase
            {
                CustomerName = "son"
            }
        };

        public static BookingTestCase GetTestData(int testCaseId)
        {
            if (_testCases.TryGetValue(testCaseId, out var testCase))
            {
                return testCase;
            }
            throw new KeyNotFoundException($"No test data found for testCaseId: {testCaseId}");
        }
    }

    public class BookingTestCase
    {
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
    }
}