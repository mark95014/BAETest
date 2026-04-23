using LDSTest.Shared;
using LDSUITest.pages;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace LDSUITest.StepDefinitions
{
    [Binding]
    public class BookingsSteps(ScenarioContext scenarioContext)
    {
        private readonly IPage _page = scenarioContext.Get<IPage>("Page");
        private readonly BookingsPage _bookingsPage = new();
        private readonly ExpectedResults _expectedResults = scenarioContext.Get<ExpectedResults>("ExpectedResults");
        private readonly Results _results = scenarioContext.Get<Results>("Results");

        [Given(@"I navigate to the bookings page")]
        public async Task GivenINavigateToTheBookingsPage()
        {
            await _bookingsPage.GoTo(_page);
        }

        [When(@"I filter bookings by customer ID ""(.*)""")]
        public async Task WhenIFilterBookingsByCustomerID(string customerId)
        {
            await _bookingsPage.FilterBookingsByCustomerId(_page, customerId);
        }

        [When(@"I filter bookings by customer name ""(.*)""")]
        public async Task WhenIFilterBookingsByCustomerName(string customerName)
        {
            await _bookingsPage.FilterBookingsByCustomerName(_page, customerName);
        }

        [Then(@"the bookings table should contain the expected data")]
        public async Task ThenThePageDataShouldBeVerifiedAgainstExpectedResults()
        {
            await _bookingsPage.VerifyPage(_page, _expectedResults, _results);
        }
    }
}