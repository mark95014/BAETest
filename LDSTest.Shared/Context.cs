using NUnit.Framework;

namespace LDSTest.Shared
{
    public static class Context
    {
        public static int GetTestCaseId()
        {
            // NUnit stores the arguments to a [TestCase] in TestContext. Arg[0] is test case id
            int testCaseId = int.Parse(TestContext.CurrentContext.Test.Arguments[0]!.ToString()!);
            return testCaseId;
        }
    }
}
