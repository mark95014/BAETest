using LDSTest.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace LDSAPITest.Utils
{
    internal class VerifyResponse
    {
        public static void Verify(string name, object response)
        {
            string dataLabel = ExpectedResults.MakeDataLabel(name, BaseApiTest.GetTestCaseId());
            JObject responseObject = JObject.FromObject(response);

            if (BaseApiTest.GenerateExpectedResults)
            {
                ExpectedResults.Append(responseObject, dataLabel);
            }
            else
            {
                string fileContent = File.ReadAllText(ExpectedResults.fileName ?? throw new InvalidOperationException("Expected results file name is null."));
                JObject expectedResults = JsonConvert.DeserializeObject<JObject>(fileContent) ?? throw new InvalidOperationException("Failed to deserialize expected results.");
                JObject expectedResult = (JObject?)expectedResults[dataLabel] ?? throw new InvalidOperationException($"Expected result not found for label: {dataLabel}");

                foreach (var property in expectedResult.Properties())
                {
                    var actualProperty = responseObject.Property(property.Name);

                    if (actualProperty == null)
                    {
                        throw new Exception($"Missing property at {dataLabel}.{property.Name}");
                    }

                    if (!JToken.DeepEquals(property.Value, actualProperty.Value))
                    {
                        throw new Exception($"Value mismatch at {dataLabel}.{property.Name}: expected '{property.Value}', got '{actualProperty.Value}'");
                    }
                }
            }
        }
    }
}
