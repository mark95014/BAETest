using LDSTest.Shared;
using LDSUITest.src.utils;
using LDSUITest.src.utils.PageData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CommonVerifyPage
{
    public static async Task Verify(BasePageData data)
    {
        await data.Get();

        string dataLabel = ExpectedResults.MakeDataLabel(data, BaseTest.GetTestCaseId());

        if (BaseTest.generateExpectedResults)
        {
            ExpectedResults.Append(data, dataLabel);
        }
        else
        {
            JObject expectedResults = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(ExpectedResults.fileName));
            JObject expectedResult = (JObject)expectedResults[dataLabel];
            await data.Verify(expectedResult, dataLabel);
        }
    }
}