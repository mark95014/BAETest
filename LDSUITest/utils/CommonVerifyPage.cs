using LDSTest.Shared;
using LDSUITest.utils.PageData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LDSUITest.utils
{
    public class CommonVerifyPage
    {
        public async Task Verify(BasePageData data, ExpectedResults expectedResults, Results results)
        {
            await data.Get();

            string dataLabel = expectedResults.MakeDataLabel(data, Context.GetTestCaseId());

            if (expectedResults.GenerateExpectedResults)
            {
                expectedResults.Append(data, dataLabel);
            }
            else
            {
                JObject _expectedResults = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(expectedResults.FileName))!;
                JObject expectedResult = (JObject)_expectedResults[dataLabel]!;
                await data.Verify(expectedResult, dataLabel, results);
            }
        }
    }
}