using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BAETest.src.utils.PageData;
using BAETest.src.utils;

public class CommonVerifyPage
{
    public static void Verify(PageData data)
    {
        data.Get();

        string dataLabel = ExpectedResults.MakeDataLabel(data, Test.GetTestCaseId());

        if (Test.generateExpectedResults)
        {
            ExpectedResults.Append(data, dataLabel);
        }
        else
        {
            JObject expectedResults = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(ExpectedResults.fileName));
            JObject expectedResult = (JObject)expectedResults[dataLabel];
            data.Verify(expectedResult, dataLabel);
        }
    }
}