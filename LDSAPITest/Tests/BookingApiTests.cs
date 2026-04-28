using FluentAssertions;
using LDSAPITest.Utils;
using LDSTest.Shared;
using NUnit.Framework;
using System.Collections;
using System.Net;
using System.Net.Http.Json;

namespace LDSAPITest.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]

    public class BookingApiTests : BaseApiTest
    {
        // Data models
        public class Booking
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public int RoomNumber { get; set; }
        }

        // Test data class
        public class BookingTestData
        {
            public int TestCaseId { get; set; }
            public string Description { get; set; } = string.Empty;
            public Booking? Booking { get; set; }
        }

        // Test data source for GetAllBookings
        private static IEnumerable GetAllBookingsTestCases
        {
            get
            {
                yield return new TestCaseData(1)
                    .SetDescription("Get all bookings");
            }
        }

        // Test data source for GetBookingById
        private static IEnumerable GetBookingByIdTestCases
        {
            get
            {
                yield return new TestCaseData(2, new Booking { Id = 1 })
                    .SetDescription("Get booking by ID 1");
                
                yield return new TestCaseData(5, new Booking { Id = 7 })
                    .SetDescription("Get booking by ID 7");
            }
        }

        // Test data source for CreateBooking
        private static IEnumerable CreateBookingTestCases
        {
            get
            {
                yield return new TestCaseData(3, new Booking 
                { 
                    Id = 0,
                    CustomerId = 7,
                    RoomNumber = 1007
                })
                .SetDescription("Create new booking");
            }
        }

        [Test]
        [TestCaseSource(nameof(GetAllBookingsTestCases))]
        public async Task GetAllBookings(int testCaseId)
        {
            var response = await GetAsync("GetAllBookings");

            await EnsureSuccessStatusCodeAsync(response);
        
            var bookings = await response.Content.ReadFromJsonAsync<List<Booking>>();
            bookings.Should().NotBeNull();
            bookings.Should().NotBeEmpty();
            VerifyResponse.Verify(new { bookings = bookings! }, ExpectedResults);
        
            LogInfo($"Retrieved {bookings?.Count} bookings");
        }

        [TestCaseSource(nameof(GetBookingByIdTestCases))]
        public async Task GetBookingById_WithValidId_ShouldReturnBooking(int testCaseId, Booking bookingInput)
        {
            int bookingId = bookingInput.Id;

            var response = await GetAsync($"GetBooking/{bookingId}");

            await AssertStatusCodeAsync(response, HttpStatusCode.OK);
        
            var booking = await response.Content.ReadFromJsonAsync<Booking>();
            VerifyResponse.Verify(new { booking = booking! }, ExpectedResults);
        }

        [NonParallelizable]
        [TestCaseSource(nameof(CreateBookingTestCases))]
        public async Task CreateBooking_WithValidData_ShouldReturnCreatedBooking(int testCaseId, Booking bookingData)
        {
            // This is necessary because the expected results for this test case reference a specific booking ID, which is generated when the booking is created.
            await new Database().ResetDatabase();
            Thread.Sleep(1000); // Wait for the database reset to complete

            var response = await PostAsync("CreateEditBooking", bookingData);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);

            response = await GetAsync("GetAllBookings");
            var bookings = await response.Content.ReadFromJsonAsync<List<Booking>>();
            VerifyResponse.Verify(new { bookings = bookings! }, ExpectedResults);

            var booking = bookings!.Find(b => b.CustomerId == bookingData.CustomerId && b.RoomNumber == bookingData.RoomNumber);
            response = await DeleteAsync($"DeleteBooking/{booking!.Id}");
            await BaseApiTest.EnsureSuccessStatusCodeAsync(response);
        }
    }
}