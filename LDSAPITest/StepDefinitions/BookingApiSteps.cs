using LDSAPITest.Utils;
using LDSTest.Shared;
using System.Net;
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
            _scenarioContext.Add("Booking", booking);
        }

        [When(@"I create a new booking")]
        public async Task WhenISendAPOSTRequestToCreateTheBooking()
        {
            var booking = _scenarioContext.Get<Booking>("Booking");
            var response = await PostAsync("CreateEditBooking", booking);
            // Must read response because it contains the generated booking ID
            booking = await response.Content.ReadFromJsonAsync<Booking>();
            await BaseApiTest.EnsureSuccessStatusCodeAsync(response);
            _scenarioContext["Booking"] = booking;
        }

        [Then(@"I delete the booking just created")]
        public async Task DeleteTheBookingJustCreated()
        {
            var booking = _scenarioContext.Get<Booking>("Booking");
            var response = await DeleteAsync($"DeleteBooking/{booking!.Id}");
            await BaseApiTest.EnsureSuccessStatusCodeAsync(response);
        }

        [When(@"I send a request to get all bookings")]
        public async Task GetAllBookings()
        {
            var response = await GetAsync("GetAllBookings");
            await BaseApiTest.AssertStatusCodeAsync(response, HttpStatusCode.OK);
            _scenarioContext["Response"] = response;

            var list = await response.Content.ReadFromJsonAsync<List<Booking>>();
            _scenarioContext["Bookings"] = list;
        }

        [When(@"I send a GET request to get booking with ID (.*)")]
        public async Task GetBookingById(int bookingId)
        {
            var response = await GetAsync($"GetBooking/{bookingId}");
            await AssertStatusCodeAsync(response, HttpStatusCode.OK);
            _scenarioContext["Response"] = response;
            _scenarioContext["Booking"] = await response.Content.ReadFromJsonAsync<Booking>();
        }

        [Then(@"the response should contain the expected booking")]
        public void ThenTheResponseShouldContainASingleBooking()
        {
            var booking = _scenarioContext.Get<Booking>("Booking");
            VerifyResponse.Verify(new { booking }, _expectedResults);
        }

        [Then(@"the response should contain the expected list of bookings")]
        public void VerifyTheListOfBookings()
        {
            var list = _scenarioContext.Get<List<Booking>>("Bookings");
            VerifyResponse.Verify(new { bookings = list }, _expectedResults);
        }

        [Then(@"the response should contain the original list of bookings plus the new booking")]
        public void VerifyModifiedListOfBookings()
        {
            VerifyTheListOfBookings();
        }
    }
}