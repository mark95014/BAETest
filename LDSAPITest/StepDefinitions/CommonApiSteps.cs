using System.Net;
using TechTalk.SpecFlow;

namespace LDSAPITest.StepDefinitions
{
    [Binding]
    public class CommonApiSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public CommonApiSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Then(@"the response status should be (.*)")]
        public async Task ThenTheResponseStatusShouldBe(HttpStatusCode httpStatusCode)
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            await BaseApiTest.AssertStatusCodeAsync(response, httpStatusCode);
        }
    }
}