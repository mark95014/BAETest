using LDSTest.Shared;
using System.Net;
using TechTalk.SpecFlow;

namespace LDSAPITest.StepDefinitions
{
    [Binding]
    public class CommonApiSteps(ScenarioContext scenarioContext)
    {
        private readonly ScenarioContext _scenarioContext = scenarioContext;

        [Then(@"the response status should be (.*)")]
        public async Task ThenTheResponseStatusShouldBe(HttpStatusCode httpStatusCode)
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            await BaseApiTest.AssertStatusCodeAsync(response, httpStatusCode);
        }

        [Then(@"I reset the database to its initial state")]
        public static async Task ResetTheDatabase()
        {
            await new Database().ResetDatabase();
        }
    }
}