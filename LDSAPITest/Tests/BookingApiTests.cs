using FluentAssertions;
using LDSAPITest.Utils;
using LDSTest.Shared;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;

namespace LDSAPITest.Examples;

/// <summary>
/// Example test class for a Booking API.
/// This demonstrates a more realistic testing scenario.
/// </summary>
[TestFixture]
public class BookingApiTests : BaseApiTest
{
    // Sample data model - replace with your actual models
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
        // Act
        var response = await GetAsync("GetAllBookings");

        // Assert
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
        JObject testInput = TestInput.GetInput(testCaseId);
        int bookingId = (int)testInput["booking"]!["Id"]!;

        // Act
        var response = await GetAsync($"GetBooking/{bookingId}");

        // Assert
        AssertStatusCode(response, HttpStatusCode.OK);
        
        //var booking = await DeserializeResponseAsync<Booking>(response);
        var booking = await response.Content.ReadFromJsonAsync<Booking>();
        //booking.Should().NotBeNull();
        //booking!.Id.Should().Be(bookingId);
        VerifyResponse.Verify(this.GetType().Name, new { booking = booking! });
    }

    [Test]
    [TestCase(3, Description = "Create new booking")]
    public async Task CreateBooking_WithValidData_ShouldReturnCreatedBooking(int testCaseId)
    {
        JObject testInput = TestInput.GetInput(testCaseId);
        JObject bookingData = testInput["booking"] as JObject ?? throw new InvalidOperationException("Booking data not found in test input");

        // Act
        var response = await PostAsync("CreateEditBooking", bookingData);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
        
        var createdBooking = await DeserializeResponseAsync<Booking>(response);
        VerifyResponse.Verify(this.GetType().Name, new { booking = createdBooking! });
    }
}