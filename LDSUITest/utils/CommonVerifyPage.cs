using LDSTest.Shared;
using LDSUITest.utils.PageData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LDSUITest.utils
{
    public class CommonVerifyPage
    {
        public void Verify(BasePageData data, ExpectedResults expectedResults, Results results)
        {
            data.Get();

            string dataLabel = expectedResults.MakeDataLabel(data);

            if (expectedResults.GenerateExpectedResults)
            {
                expectedResults.Append(data, dataLabel);
            }
            else
            {
                JObject _expectedResults = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(expectedResults.FileName))!;
                JObject expectedResult = (JObject)_expectedResults[dataLabel]!;
                data.Verify(expectedResult, dataLabel, results);
            }
        }
    }
}