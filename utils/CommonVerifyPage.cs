using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BAETest.src.utils.PageData;
using BAETest.src.utils;
using System.Threading.Tasks;

public class CommonVerifyPage
{
    public static async Task Verify(BasePageData data)
    {
        await data.Get();

        string dataLabel = ExpectedResults.MakeDataLabel(data, BaseTest.GetTestCaseId());

        if (Test.generateExpectedResults)
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