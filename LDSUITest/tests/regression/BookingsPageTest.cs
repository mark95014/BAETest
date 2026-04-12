using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    public class BookingsPageTest : BaseTest
    {
        private BookingsPage _bookingsPage = null!;

        [SetUp]
        public async Task Setup()
        {
            _bookingsPage = new BookingsPage();
            await _bookingsPage.GoTo(Page);
        }

        [TestCase(1, Description = "Verify all data on Bookings page")]
        public async Task VerifyBookingsPage(int testCaseId)
        {
            await BasePage.VerifyPage<BookingsPageData>(Page);
        }

        [TestCase(2, "7", Description = "Verify filter functionality on Bookings page")]
        public async Task VerifyBookingsFilter(int testCaseId, string customerId)
        {
            await _bookingsPage.FilterBookings(Page, customerId);
            await BasePage.VerifyPage<BookingsPageData>(Page);
        }

        [TestCase(3, "son", Description = "Verify filter by customer name containing 'son'")]
        public async Task VerifyBookingsFilterByCustomerName(int testCaseId, string customerName)
        {
            await _bookingsPage.FilterBookingsByCustomerName(Page, customerName);
            await BasePage.VerifyPage<BookingsPageData>(Page);
        }
    }
}