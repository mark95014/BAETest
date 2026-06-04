using LDSAPITest;
using LDSTest.Shared;
using LDSUITest.pages;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace LDSUITest.StepDefinitions
{
    [Binding]
    public class BookingsSteps(ScenarioContext scenarioContext) : BaseApiTest
    {
        private readonly IWebDriver _driver = scenarioContext.Get<IWebDriver>("Driver");
        private readonly BookingsPage _bookingsPage = new();
        private readonly ExpectedResults _expectedResults = scenarioContext.Get<ExpectedResults>("ExpectedResults");
        private readonly Results _results = scenarioContext.Get<Results>("Results");

        [Given(@"I navigate to the bookings page")]
        public void GivenINavigateToTheBookingsPage()
        {
            _bookingsPage.GoTo(_driver);
        }

        [When(@"I filter bookings by customer ID ""(.*)""")]
        public void WhenIFilterBookingsByCustomerID(string customerId)
        {
            _bookingsPage.FilterBookingsByCustomerId(_driver, customerId);
        }

        [When(@"I filter bookings by customer name ""(.*)""")]
        public void WhenIFilterBookingsByCustomerName(string customerName)
        {
            _bookingsPage.FilterBookingsByCustomerName(_driver, customerName);
        }

        [Then(@"the bookings table should contain the expected data")]
        public void ThenThePageDataShouldBeVerifiedAgainstExpectedResults()
        {
            _bookingsPage.VerifyPage(_driver, _expectedResults, _results);
        }
    }
}