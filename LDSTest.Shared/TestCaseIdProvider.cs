using NUnit.Framework;

namespace LDSTest.Shared
{
    /// <summary>
    /// Provides access to the current test case ID for both NUnit and SpecFlow tests.
    /// </summary>
    public static class TestCaseIdProvider
    {
        [ThreadStatic]
        private static int? _overrideTestCaseId;

        /// <summary>
        /// Set the test case ID explicitly (used by SpecFlow tests).
        /// </summary>
        public static void SetTestCaseId(int testCaseId)
        {
            _overrideTestCaseId = testCaseId;
        }

        /// <summary>
        /// Clear the test case ID (called after test completion).
        /// </summary>
        public static void Clear()
        {
            _overrideTestCaseId = null;
        }

        /// <summary>
        /// Get the current test case ID.
        /// For SpecFlow: Returns the value set via SetTestCaseId()
        /// For NUnit: Extracts from TestContext.CurrentContext.Test.Arguments[0]
        /// </summary>
        public static int GetTestCaseId()
        {
            // If explicitly set (e.g., by SpecFlow hooks), use that value
            if (_overrideTestCaseId.HasValue)
            {
                return _overrideTestCaseId.Value;
            }

            // Otherwise, try to get from NUnit TestContext Arguments
            if (TestContext.CurrentContext.Test.Arguments != null && 
                TestContext.CurrentContext.Test.Arguments.Length > 0)
            {
                if (int.TryParse(TestContext.CurrentContext.Test.Arguments[0]?.ToString(), out int testCaseId))
                {
                    return testCaseId;
                }
            }

            // Default: no test case ID
            return 0;
        }
    }
}