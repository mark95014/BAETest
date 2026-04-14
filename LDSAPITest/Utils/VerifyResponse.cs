using LDSTest.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace LDSAPITest.Utils
{
    internal class VerifyResponse
    {
        public static void Verify(object response, ExpectedResults expectedResults)
        {
            string dataLabel = expectedResults.MakeDataLabel(TestContext.CurrentContext.Test.Name, int.Parse(TestContext.CurrentContext.Test.ID));
            JObject responseObject = JObject.FromObject(response);

            if (expectedResults.GenerateExpectedResults)
            {
                expectedResults.Append(responseObject, dataLabel);
            }
            else
            {
                string fileContent = File.ReadAllText(expectedResults.FileName ?? throw new InvalidOperationException("Expected results file name is null."));
                JObject _expectedResults = JsonConvert.DeserializeObject<JObject>(fileContent) ?? throw new InvalidOperationException("Failed to deserialize expected results.");
                JObject expectedResult = (JObject?)_expectedResults[dataLabel] ?? throw new InvalidOperationException($"Expected result not found for label: {dataLabel}");

                foreach (var property in expectedResult.Properties())
                {
                    var actualProperty = responseObject.Property(property.Name) ?? throw new Exception($"Missing property at {dataLabel}.{property.Name}");
                    // Implement use of Results class for better error reporting
                    if (!JToken.DeepEquals(property.Value, actualProperty.Value))
                    {
                        throw new Exception($"Value mismatch at {dataLabel}.{property.Name}: expected '{property.Value}', got '{actualProperty.Value}'");
                    }
                }
            }
        }
    }
}