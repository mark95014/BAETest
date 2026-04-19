using FluentAssertions;
using LDSTest.Shared;
using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace LDSUITest.StepDefinitions
{
    [Binding]
    public class BookingsSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private IPage _page = null!;
        private BookingsPage _bookingsPage = null!;
        private ExpectedResults _expectedResults = null!;
        private Results _results = null!;

        public BookingsSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"the hotel booking application is running")]
        public void GivenTheHotelBookingApplicationIsRunning()
        {
            // Application is already running - browser is initialized in hooks
            _page = _scenarioContext.Get<IPage>("Page");
            _page.Should().NotBeNull("Page should be initialized");
        }

        [Given(@"the database has been reset")]
        public async Task GivenTheDatabaseHasBeenReset()
        {
            await new Database().ResetDatabase();
        }

        [Given(@"I am on the bookings page")]
        public async Task GivenIAmOnTheBookingsPage()
        {
            _page = _scenarioContext.Get<IPage>("Page");
            _expectedResults = _scenarioContext.Get<ExpectedResults>("ExpectedResults");
            _results = _scenarioContext.Get<Results>("Results");
            
            _bookingsPage = new BookingsPage();
            await _bookingsPage.GoTo(_page);
        }

        [When(@"I filter bookings by customer ID ""(.*)""")]
        public async Task WhenIFilterBookingsByCustomerID(string customerId)
        {
            await _bookingsPage.FilterBookings(_page, customerId);
            _scenarioContext["FilteredCustomerId"] = customerId;
        }

        [When(@"I filter bookings by customer name ""(.*)""")]
        public async Task WhenIFilterBookingsByCustomerName(string customerName)
        {
            await _bookingsPage.FilterBookingsByCustomerName(_page, customerName);
            _scenarioContext["FilteredCustomerName"] = customerName;
        }

        [Then(@"the bookings page should display correctly")]
        public async Task ThenTheBookingsPageShouldDisplayCorrectly()
        {
            var pageTitle = _page.Locator("h1:has-text('All Bookings')");
            await pageTitle.WaitForAsync();
            (await pageTitle.IsVisibleAsync()).Should().BeTrue("Page title 'All Bookings' should be visible");
            
            var bookingsTable = _page.Locator("[id='bookingsTable']");
            (await bookingsTable.IsVisibleAsync()).Should().BeTrue("Bookings table should be visible");
        }

        [Then(@"the page data should be verified against expected results")]
        public async Task ThenThePageDataShouldBeVerifiedAgainstExpectedResults()
        {
            await BasePage.VerifyPage<BookingsPageData>(_page, _expectedResults, _results);
        }
    }
}