namespace LDSUITest.data.TestInput
{
    public static class CustomersPageTestData
    {
        // Dictionary to look up test data by testCaseId
        private static readonly Dictionary<int, CustomerTestCase> _testCases = new()
        {
            [2] = new CustomerTestCase
            {
                CustomerId = "7"
            },
            [3] = new CustomerTestCase
            {
                CustomerName = "son"
            }
        };

        public static CustomerTestCase GetTestData(int testCaseId)
        {
            if (_testCases.TryGetValue(testCaseId, out var testCase))
            {
                return testCase;
            }
            throw new KeyNotFoundException($"No test data found for testCaseId: {testCaseId}");
        }
    }

    public class CustomerTestCase
    {
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
    }
}