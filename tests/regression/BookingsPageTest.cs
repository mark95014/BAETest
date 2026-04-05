using BAETest.pages;
using BAETest.src.utils;
using BAETest.utils.PageData;
using NUnit.Framework;

namespace BAETest.tests.regression
{
    [TestFixture]
    public class BookingsPageTest : BaseTest
    {
        private BookingsPageData _pageData;

        [SetUp]
        public async Task Setup()
        {
            await Page.GotoAsync(BookingsPage.url);

            _pageData = new BookingsPageData();
            _pageData.Initialize(Page);
        }

        [TestCase(1, Description = "Verify all data on Bookings page")]
        public void VerifyBookingsPage(int testCaseId)
        {
            BookingsPage.VerifyPage();
        }

        [TestCase(2, Description = "Verify filter functionality on Bookings page")]
        public async Task VerifyBookingsFilter(int testCaseId)
        {
            await BookingsPage.FilterBookings(Page, "7");
            BookingsPage.VerifyPage();
        }
    }
}