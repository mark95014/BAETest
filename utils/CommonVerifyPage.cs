using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BAETest.src.utils.PageData;
using BAETest.src.utils;

public class CommonVerifyPage
{
    public static void Verify(BasePageData data)
    {
        data.Get();

        string dataLabel = ExpectedResults.MakeDataLabel(data, BaseTest.GetTestCaseId());

        if (BaseTest.generateExpectedResults)
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