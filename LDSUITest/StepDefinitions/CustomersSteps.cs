using LDSTest.Shared;
using LDSUITest.pages;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace LDSUITest.StepDefinitions
{
    [Binding]
    public class CustomersSteps(ScenarioContext scenarioContext)
    {
        private readonly IPage _page = scenarioContext.Get<IPage>("Page");
        private readonly CustomersPage _customersPage = new();
        private readonly ExpectedResults _expectedResults = scenarioContext.Get<ExpectedResults>("ExpectedResults");
        private readonly Results _results = scenarioContext.Get<Results>("Results");

        [Given(@"I navigate to the customers page")]
        public async Task GivenINavigateToTheCustomersPage()
        {
            await _customersPage.GoTo(_page);
        }

        [Then(@"the customers table should contain the expected data")]
        public async Task ThenTheCustomersTableShouldContainTheExpectedData()
        {
            await _customersPage.VerifyPage(_page, _expectedResults, _results);
        }

        [When(@"I filter customers by ID ""(.*)""")]
        public async Task WhenIFilterCustomersByID(string customerId)
        {
            await _customersPage.FilterCustomersById(_page, customerId);
        }

        [When(@"I filter customers by name ""(.*)""")]
        public async Task WhenIFilterCustomersByName(string customerName)
        {
            await _customersPage.FilterCustomersByName(_page, customerName);
        }
    }
}