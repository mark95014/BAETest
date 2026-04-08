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
    [TestCase(2, 1, Description = "Get booking by ID")]
    [TestCase(5, 7, Description = "Get booking by ID")]
    public async Task GetBookingById_WithValidId_ShouldReturnBooking(int testCaseId, int bookingId)
    {
        JObject testInput = TestInput.GetInput(testCaseId);
        int id = (int)testInput["booking"]!["Id"]!;

        // Act
        var response = await GetAsync($"GetBooking/{id}");

        // Assert
        AssertStatusCode(response, HttpStatusCode.OK);
        
        //var booking = await DeserializeResponseAsync<Booking>(response);
        var booking = await response.Content.ReadFromJsonAsync<Booking>();
        booking.Should().NotBeNull();
        booking!.Id.Should().Be(bookingId);
        VerifyResponse.Verify(this.GetType().Name, new { booking = booking! });
    }

    [Test]
    [TestCase(3, Description = "Create new booking")]
    public async Task CreateBooking_WithValidData_ShouldReturnCreatedBooking(int testCaseId)
    {
        // Arrange
        var newBooking = new
        {
            Id = 0,
            CustomerId = 7,
            RoomNumber = 1007
        };

        // Act
        var response = await PostAsync("CreateEditBooking", newBooking);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
        
        var createdBooking = await DeserializeResponseAsync<Booking>(response);
        createdBooking.Should().NotBeNull();
        createdBooking!.CustomerId.Should().Be(newBooking.CustomerId);
        createdBooking.RoomNumber.Should().Be(newBooking.RoomNumber);
        VerifyResponse.Verify(this.GetType().Name, new { booking = createdBooking! });
    }
}