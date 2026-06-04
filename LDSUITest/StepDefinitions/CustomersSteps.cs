using LDSTest.Shared;
using LDSUITest.pages;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace LDSUITest.StepDefinitions
{
    [Binding]
    public class CustomersSteps(ScenarioContext scenarioContext)
    {
        private readonly IWebDriver _driver = scenarioContext.Get<IWebDriver>("Driver");
        private readonly CustomersPage _customersPage = new();
        private readonly ExpectedResults _expectedResults = scenarioContext.Get<ExpectedResults>("ExpectedResults");
        private readonly Results _results = scenarioContext.Get<Results>("Results");

        [Given(@"I navigate to the customers page")]
        public void GivenINavigateToTheCustomersPage()
        {
            _customersPage.GoTo(_driver);
        }

        [Then(@"the customers table should contain the expected data")]
        public void ThenTheCustomersTableShouldContainTheExpectedData()
        {
            _customersPage.VerifyPage(_driver, _expectedResults, _results);
        }

        [When(@"I filter customers by ID ""(.*)""")]
        public void WhenIFilterCustomersByID(string customerId)
        {
            _customersPage.FilterCustomersById(_driver, customerId);
        }

        [When(@"I filter customers by name ""(.*)""")]
        public void WhenIFilterCustomersByName(string customerName)
        {
            _customersPage.FilterCustomersByName(_driver, customerName);
        }
    }
}