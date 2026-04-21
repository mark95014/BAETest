using NUnit.Framework;

namespace LDSTest.Shared
{
    /// <summary>
    /// Provides access to the current test name for both NUnit and SpecFlow tests.
    /// </summary>
    public static class TestNameProvider
    {
        [ThreadStatic]
        private static string? _overrideTestName;

        /// <summary>
        /// Set the test name explicitly (used by SpecFlow tests).
        /// Should be set to the feature title from the .feature file.
        /// </summary>
        /// <param name="testName">The test name exactly as written in the feature file (without "Feature:" prefix)</param>
        public static void SetTestName(string testName)
        {
            _overrideTestName = testName;
        }

        /// <summary>
        /// Clear the test name (called after test completion).
        /// </summary>
        public static void Clear()
        {
            _overrideTestName = null;
        }

        /// <summary>
        /// Get the current test name.
        /// For SpecFlow: Returns the value set via SetTestName() (the feature title)
        /// For NUnit: Returns TestContext.CurrentContext.Test.Name
        /// </summary>
        public static string GetTestName()
        {
            // If explicitly set (e.g., by SpecFlow hooks), use that value
            if (!string.IsNullOrEmpty(_overrideTestName))
            {
                return _overrideTestName;
            }

            // Otherwise, get from NUnit TestContext
            return TestContext.CurrentContext.Test.Name;
        }
    }
}