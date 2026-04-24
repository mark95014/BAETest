using LDSAPITest.Utils;
using LDSTest.Shared;
using System.Net.Http.Json;
using TechTalk.SpecFlow;

namespace LDSAPITest.StepDefinitions
{
    [Binding]
    public class BookingApiSteps : BaseApiTest
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly ExpectedResults _expectedResults = null!;

        public class Booking
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public int RoomNumber { get; set; }
        }

        public BookingApiSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _expectedResults = _scenarioContext.Get<ExpectedResults>("ExpectedResults");
            BaseOneTimeSetUp();
        }

        [Given(@"I have a new booking with the following details:")]
        public void GivenIHaveANewBookingWithTheFollowingDetails(Table table)
        {
            var booking = new Booking
            {
                Id = 0,
                CustomerId = int.Parse(table.Rows[0]["Value"]),
                RoomNumber = int.Parse(table.Rows[1]["Value"])
            };
            _scenarioContext.Add("NewBooking", booking);
        }

        [When(@"I create a new booking")]
        public async Task WhenISendAPOSTRequestToCreateTheBooking()
        {
            var booking = _scenarioContext.Get<Booking>("NewBooking");
            _scenarioContext["Response"] = await PostAsync("CreateEditBooking", booking);
        }

        [When(@"I send a request to get all bookings")]
        public async Task GetAllBookings()
        {
            _scenarioContext["Response"] = await GetAsync("GetAllBookings");
        }

        [When(@"I send a GET request to get booking with ID (.*)")]
        public async Task GetBookingById(int bookingId)
        {
            _scenarioContext["Response"] = await GetAsync($"GetBooking/{bookingId}");
        }

        [Then(@"the response should contain the expected booking")]
        public async Task ThenTheResponseShouldContainASingleBooking()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var booking = await response.Content.ReadFromJsonAsync<Booking>();
            VerifyResponse.Verify(new { booking }, _expectedResults);
        }

        [Then(@"the response should contain the expected bookings")]
        public async Task ThenTheResponseShouldContainAListOfBookings()
        {

            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var list = await response.Content.ReadFromJsonAsync<List<Booking>>();
            VerifyResponse.Verify(new { bookings = list }, _expectedResults);
        }
    }
}