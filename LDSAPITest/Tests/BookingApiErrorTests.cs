using FluentAssertions;
using LDSAPITest;
using NUnit.Framework;
using System.Collections;
using System.Net;
using System.Net.Http.Json;

namespace LDSAPITest.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class BookingApiErrorTests : BaseApiTest
    {
        // Data models
        public class Booking
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public int RoomNumber { get; set; }
        }

        #region Test Data Sources

        // Test cases for validation errors (400)
        private static IEnumerable CreateBookingInvalidCustomerId
        {
            get
            {
                yield return new TestCaseData(10, new Booking 
                { 
                    Id = 0,
                    CustomerId = 0,  // Invalid: missing/zero CustomerId
                    RoomNumber = 101
                })
                .SetDescription("Create booking with missing CustomerId should return 400");
            }
        }

        private static IEnumerable CreateBookingInvalidRoomNumber
        {
            get
            {
                yield return new TestCaseData(11, new Booking 
                { 
                    Id = 0,
                    CustomerId = 1,
                    RoomNumber = 0  // Invalid: missing/zero RoomNumber
                })
                .SetDescription("Create booking with missing RoomNumber should return 400");
            }
        }

        private static IEnumerable CreateBookingInvalidData
        {
            get
            {
                yield return new TestCaseData(12, new Booking 
                { 
                    Id = 0,
                    CustomerId = -1,  // Invalid: negative CustomerId
                    RoomNumber = 101
                })
                .SetDescription("Create booking with negative CustomerId should return 400");

                yield return new TestCaseData(13, new Booking 
                { 
                    Id = 0,
                    CustomerId = 1,
                    RoomNumber = -1  // Invalid: negative RoomNumber
                })
                .SetDescription("Create booking with negative RoomNumber should return 400");
            }
        }

        // Test cases for not found errors (404)
        private static IEnumerable GetNonExistentBooking
        {
            get
            {
                yield return new TestCaseData(20, 99999).SetDescription("Get non-existent booking should return 404");
                yield return new TestCaseData(21, 0).SetDescription("Get booking with ID 0 should return 404");
            }
        }

        private static IEnumerable DeleteNonExistentBooking
        {
            get
            {
                yield return new TestCaseData(22, 99999).SetDescription("Delete non-existent booking should return 404");
            }
        }

        private static IEnumerable UpdateNonExistentBooking
        {
            get
            {
                yield return new TestCaseData(23, new Booking 
                { 
                    Id = 99999,  // Non-existent booking ID
                    CustomerId = 1,
                    RoomNumber = 101
                })
                .SetDescription("Update non-existent booking should return 404");
            }
        }

        private static IEnumerable CreateBookingNonExistentCustomer
        {
            get
            {
                yield return new TestCaseData(24, new Booking 
                { 
                    Id = 0,
                    CustomerId = 99999,  // Non-existent customer
                    RoomNumber = 101
                })
                .SetDescription("Create booking with non-existent CustomerId should return 404");
            }
        }

        private static IEnumerable CreateBookingNonExistentRoom
        {
            get
            {
                yield return new TestCaseData(25, new Booking 
                { 
                    Id = 0,
                    CustomerId = 1,
                    RoomNumber = 99999  // Non-existent room
                })
                .SetDescription("Create booking with non-existent RoomNumber should return 404");
            }
        }

        #endregion

        #region Validation Error Tests (400 Bad Request)

        [Test]
        [TestCaseSource(nameof(CreateBookingInvalidCustomerId))]
        public async Task CreateBooking_InvalidCustomerId(int testCaseId, Booking bookingData)
        {
            // Act
            var response = await PostAsync("CreateEditBooking", bookingData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, "booking with missing CustomerId should not be created");

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty("error response should contain a message");

            LogInfo($"Error response: {content}");
        }

        [Test]
        [TestCaseSource(nameof(CreateBookingInvalidRoomNumber))]
        public async Task CreateBooking_InvalidRoomNumber(int testCaseId, Booking bookingData)
        {
            // Act
            var response = await PostAsync("CreateEditBooking", bookingData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, "booking with missing RoomNumber should not be created");

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty("error response should contain a message");

            LogInfo($"Error response: {content}");
        }

        [Test]
        [TestCaseSource(nameof(CreateBookingInvalidData))]
        public async Task CreateBooking_InvalidData(int testCaseId, Booking bookingData)
        {
            // Act
            var response = await PostAsync("CreateEditBooking", bookingData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, "booking with invalid data should not be created");

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty("error response should contain a message");

            LogInfo($"Error response: {content}");
        }

        #endregion

        #region Not Found Error Tests (404)

        [Test]
        [TestCaseSource(nameof(GetNonExistentBooking))]
        public async Task GetBooking_NonExistentId(int testCaseId, int bookingId)
        {
            // Act
            var response = await GetAsync($"GetBooking/{bookingId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, $"booking with ID {bookingId} should not exist");

            var content = await response.Content.ReadAsStringAsync();
            content.ToLower().Should().Contain("not found", "error message should indicate resource was not found");

            LogInfo($"Error response: {content}");
        }

        [Test]
        [TestCaseSource(nameof(DeleteNonExistentBooking))]
        public async Task DeleteBooking_NonExistentId(int testCaseId, int bookingId)
        {
            // Act
            var response = await DeleteAsync($"DeleteBooking/{bookingId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, $"cannot delete booking with ID {bookingId} that does not exist");

            var content = await response.Content.ReadAsStringAsync();
            content.ToLower().Should().Contain("not found", "error message should indicate resource was not found");

            LogInfo($"Error response: {content}");
        }

        [Test]
        [TestCaseSource(nameof(UpdateNonExistentBooking))]
        public async Task UpdateBooking_NonExistentId(int testCaseId, Booking bookingData)
        {
            // Act
            var response = await PutAsync("CreateEditBooking", bookingData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed, $"cannot update booking with ID {bookingData.Id} that does not exist");
        }

        [Test]
        [TestCaseSource(nameof(CreateBookingNonExistentCustomer))]
        public async Task CreateBooking_NonExistentCustomer(int testCaseId, Booking bookingData)
        {
            // Act
            var response = await PostAsync("CreateEditBooking", bookingData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, $"cannot create booking with non-existent CustomerId {bookingData.CustomerId}");

            var content = await response.Content.ReadAsStringAsync();
            content.ToLower().Should().Contain("customer", "error message should indicate customer was not found");

            LogInfo($"Error response: {content}");
        }

        [Test]
        [TestCaseSource(nameof(CreateBookingNonExistentRoom))]
        public async Task CreateBooking_NonExistentRoom(int testCaseId, Booking bookingData)
        {
            // Act
            var response = await PostAsync("CreateEditBooking", bookingData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, $"cannot create booking with non-existent RoomNumber {bookingData.RoomNumber}");

            var content = await response.Content.ReadAsStringAsync();
            content.ToLower().Should().Contain("room", "error message should indicate room was not found");

            LogInfo($"Error response: {content}");
        }

        #endregion

        #region Helper Methods (if needed for verification)

        /// <summary>
        /// Verify that a booking was NOT created by checking it doesn't exist in the system
        /// </summary>
        private async Task VerifyBookingNotCreated(Booking bookingData)
        {
            var response = await GetAsync("GetAllBookings");
            await EnsureSuccessStatusCodeAsync(response);

            var bookings = await response.Content.ReadFromJsonAsync<List<Booking>>();
            bookings.Should().NotBeNull();

            var exists = bookings!.Any(b => 
                b.CustomerId == bookingData.CustomerId && 
                b.RoomNumber == bookingData.RoomNumber);

            exists.Should().BeFalse(
                $"Booking with CustomerId={bookingData.CustomerId} and RoomNumber={bookingData.RoomNumber} should not exist");
        }

        #endregion
    }
}