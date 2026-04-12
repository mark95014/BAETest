using System.Diagnostics;

namespace LDSUITest.utils
{
    public class Result
    {
        public bool passed;
        public string message;
        public int testCaseId;
        public List<string> stackTrace = new List<string>();

        public Result(bool passed, string message = "")
        {
            this.passed = passed;
            this.message = message;
            this.testCaseId = BaseTest.GetTestCaseId();

            if (!passed)
            {
                string currentMethod = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
                string currentNameSpace = GetType().Namespace!.ToString().Split(".")[0];
                StackFrame[] frames = new StackTrace(true).GetFrames()!;

                foreach (StackFrame frame in frames)
                {
                    string frameMethod = frame.GetMethod()!.Name;
                    string frameFileName = frame.GetFileName() ?? "";

                    if (!frameMethod.Equals(currentMethod) && frameFileName.Contains(currentNameSpace))
                    {
                        stackTrace.Add(frame.ToString());
                    }
                }
            }
        }
    }
}