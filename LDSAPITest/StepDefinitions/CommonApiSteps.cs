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

        [Then(@"the response status should be successful")]
        public async Task ThenTheResponseStatusShouldBeSuccessful()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            await BaseApiTest.EnsureSuccessStatusCodeAsync(response);
        }
    }
}