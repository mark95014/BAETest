using FluentAssertions;
using LDSAPITest.Utils;
using LDSTest.Shared;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace LDSAPITest.StepDefinitions
{
    [Binding]
    public class BookingApiSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private BaseApiTest _baseApiTest = null!;
        private HttpResponseMessage _response = null!;
        private ExpectedResults _expectedResults = null!;

        public class Booking
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public int RoomNumber { get; set; }
        }

        public BookingApiSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"the API is available")]
        public void GivenTheAPIIsAvailable()
        {
            _baseApiTest = _scenarioContext.Get<BaseApiTest>("BaseApiTest");
            _expectedResults = _scenarioContext.Get<ExpectedResults>("ExpectedResults");
            _baseApiTest.Should().NotBeNull();
            _expectedResults.Should().NotBeNull();
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

        [When(@"I send a GET request to ""(.*)""")]
        public async Task WhenISendAGETRequestTo(string endpoint)
        {
            _response = await _baseApiTest.GetAsync(endpoint);
            _scenarioContext.Add("Response", _response);
        }

        [When(@"I send a POST request to ""(.*)""")]
        public async Task WhenISendAPOSTRequestTo(string endpoint)
        {
            var booking = _scenarioContext.Get<Booking>("NewBooking");
            _response = await _baseApiTest.PostAsync(endpoint, booking);
            _scenarioContext.Add("Response", _response);
        }

        [Then(@"the response status should be (.*)")]
        public async Task ThenTheResponseStatusShouldBe(int expectedStatusCode)
        {
            _response = _scenarioContext.Get<HttpResponseMessage>("Response");
            await BaseApiTest.AssertStatusCodeAsync(_response, (HttpStatusCode)expectedStatusCode);
        }

        [Then(@"the response should contain a list of bookings")]
        public async Task ThenTheResponseShouldContainAListOfBookings()
        {
            var bookings = await _response.Content.ReadFromJsonAsync<List<Booking>>();
            bookings.Should().NotBeNull();
            bookings.Should().NotBeEmpty();
            
            // USE YOUR EXISTING ExpectedResults PATTERN!
            VerifyResponse.Verify(new { bookings = bookings! }, _expectedResults);
            
            _baseApiTest.LogInfo($"Retrieved {bookings?.Count} bookings");
        }

        [Then(@"the response should contain booking details for ID (.*)")]
        public async Task ThenTheResponseShouldContainBookingDetailsForID(int bookingId)
        {
            var booking = await _response.Content.ReadFromJsonAsync<Booking>();
            booking.Should().NotBeNull();
            
            // USE YOUR EXISTING ExpectedResults PATTERN!
            VerifyResponse.Verify(new { booking = booking! }, _expectedResults);
        }

        [Then(@"the created booking should have customer ID (.*)")]
        public async Task ThenTheCreatedBookingShouldHaveCustomerID(int customerId)
        {
            var createdBooking = await _response.Content.ReadFromJsonAsync<Booking>();
            
            // USE YOUR EXISTING ExpectedResults PATTERN!
            VerifyResponse.Verify(new { booking = createdBooking! }, _expectedResults);
        }

        [Then(@"the created booking should have room number (.*)")]
        public async Task ThenTheCreatedBookingShouldHaveRoomNumber(int roomNumber)
        {
            // This is already verified in the previous step since we verify the whole object
            // But we can add specific assertions if needed
            var booking = await _response.Content.ReadFromJsonAsync<Booking>();
            booking!.RoomNumber.Should().Be(roomNumber);
        }
    }
}