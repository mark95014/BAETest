using FluentAssertions;
using LDSAPITest.data.TestInput;
using LDSAPITest.Utils;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;

namespace LDSAPITest.Tests
{
    [TestFixture]
    public class BookingApiTests : BaseApiTest
    {
        // Data models
        public class Booking
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public int RoomNumber { get; set; }
        }

        [Test]
        [TestCase(1, Description = "Get all bookings")]
        public async Task GetAllBookings_ShouldReturnListOfBookings(int testCaseId)
        {
            var response = await GetAsync("GetAllBookings");

            await EnsureSuccessStatusCodeAsync(response);
        
            var bookings = await DeserializeResponseAsync<List<Booking>>(response);
            bookings.Should().NotBeNull();
            bookings.Should().NotBeEmpty();
            VerifyResponse.Verify(this.GetType().Name, new { bookings = bookings! });
        
            LogInfo($"Retrieved {bookings?.Count} bookings");
        }

        [Test]
        [TestCase(2, Description = "Get booking by ID 1")]
        [TestCase(5, Description = "Get booking by ID 7")]
        public async Task GetBookingById_WithValidId_ShouldReturnBooking(int testCaseId)
        {
            var testData = BookingApiTestData.GetTestData(testCaseId);
            int bookingId = testData.Booking.Id;

            var response = await GetAsync($"GetBooking/{bookingId}");

            AssertStatusCode(response, HttpStatusCode.OK);
        
            var booking = await response.Content.ReadFromJsonAsync<Booking>();
            VerifyResponse.Verify(this.GetType().Name, new { booking = booking! });
        }

        [Test]
        [TestCase(3, Description = "Create new booking")]
        public async Task CreateBooking_WithValidData_ShouldReturnCreatedBooking(int testCaseId)
        {
            var testData = BookingApiTestData.GetTestData(testCaseId);
            var bookingData = testData.Booking;

            var response = await PostAsync("CreateEditBooking", bookingData);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
        
            var createdBooking = await DeserializeResponseAsync<Booking>(response);
            VerifyResponse.Verify(this.GetType().Name, new { booking = createdBooking! });
        }
    }
}