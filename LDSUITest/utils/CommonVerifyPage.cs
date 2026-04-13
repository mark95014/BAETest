using LDSTest.Shared;
using LDSUITest.utils.PageData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LDSUITest.utils
{
    public class CommonVerifyPage
    {
        public static async Task Verify(BasePageData data)
        {
            await data.Get();

            string dataLabel = ExpectedResults.MakeDataLabel(data, Context.GetTestCaseId());

            if (BaseTest.GenerateExpectedResults)
            {
                ExpectedResults.Append(data, dataLabel);
            }
            else
            {
                JObject expectedResults = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(ExpectedResults.FileName))!;
                JObject expectedResult = (JObject)expectedResults[dataLabel]!;
                await data.Verify(expectedResult, dataLabel);
            }
        }
    }
}