using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

namespace LDSTest.Shared
{
    // This class is an obsolete utility. See LDS*Test/data/TestInput for the new way.
    public class TestInput
    {
        public static JObject GetInput(int testCaseId, [CallerFilePath] string callerFilePath = "")
        {
            string testname = Path.GetFileNameWithoutExtension(callerFilePath);
            string fileName = "../../../data/TestInput/" + testname + "Input.json";
            var fileContent = File.ReadAllText(fileName);
            JObject allTestInput = JObject.Parse(fileContent);
            string index = testname + "." + testCaseId.ToString();
            JObject testCaseInput = (allTestInput[index] as JObject)!;
            return testCaseInput;
        }
    }
}