using LDSTest.pages;
using LDSTest.src.utils;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using BookingsData = LDSTest.utils.PageData.BookingsPageData;

namespace LDSTest.tests.regression
{
    [TestFixture]
    public class BookingsPageTest : BaseTest
    {
        private BookingsData _bookingsPageData;

        [SetUp]
        public async Task Setup()
        {
            await Page.GotoAsync("https://localhost:7031/bookings");

            _bookingsPageData = new BookingsData();
            _bookingsPageData.Initialize(Page);
        }

        [TestCase(1, Description = "Verify all data on Bookings page")]
        public async Task VerifyBookingsPage(int testCaseId)
        {
            await BookingsPage.VerifyPage(Page);
        }

        [TestCase(2, Description = "Verify filter functionality on Bookings page")]
        public async Task VerifyBookingsFilter(int testCaseId)
        {
            await BookingsPage.FilterBookings(Page, "7");
            await BookingsPage.VerifyPage(Page);
        }

        [TestCase(3, Description = "Verify filter by customer name containing 'son'")]
        public async Task VerifyBookingsFilterByCustomerName(int testCaseId)
        {
            await BookingsPage.FilterBookingsByCustomerName(Page, "son");
            await BookingsPage.VerifyPage(Page);
        }
    }
}