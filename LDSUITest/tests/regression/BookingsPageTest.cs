using LDSUITest.pages;
using LDSUITest.src.utils;
using NUnit.Framework;
using BookingsData = LDSUITest.utils.PageData.BookingsPageData;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    public class BookingsPageTest : BaseTest
    {
        private BookingsData _bookingsPageData;

        [SetUp]
        public async Task Setup()
        {
            await BookingsPage.GoTo(Page);

            _bookingsPageData = new BookingsData();
            _bookingsPageData.Initialize(Page);
        }

        [TestCase(1, Description = "Verify all data on Bookings page")]
        public async Task VerifyBookingsPage(int testCaseId)
        {
            await BookingsPage.VerifyPage(Page);
        }

        [TestCase(2, "7", Description = "Verify filter functionality on Bookings page")]
        public async Task VerifyBookingsFilter(int testCaseId, string customerId)
        {
            await BookingsPage.FilterBookings(Page, customerId);
            await BookingsPage.VerifyPage(Page);
        }

        [TestCase(3, "son", Description = "Verify filter by customer name containing 'son'")]
        public async Task VerifyBookingsFilterByCustomerName(int testCaseId, string customerName)
        {
            await BookingsPage.FilterBookingsByCustomerName(Page, customerName);
            await BookingsPage.VerifyPage(Page);
        }
    }
}