using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;
using System.Collections;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]

    public class BookingsPageTest : BaseTest
    {
        private BookingsPage _bookingsPage = null!;

        // Test data source for VerifyBookingsPage
        private static IEnumerable VerifyBookingsPageTestCases
        {
            get
            {
                yield return new TestCaseData(1)
                    .SetDescription("Verify all data on Bookings page");
            }
        }

        // Test data source for VerifyBookingsFilter
        private static IEnumerable VerifyBookingsFilterTestCases
        {
            get
            {
                yield return new TestCaseData(2, "7")
                    .SetDescription("Verify filter functionality on Bookings page");
            }
        }

        // Test data source for VerifyBookingsFilterByCustomerName
        private static IEnumerable VerifyBookingsFilterByCustomerNameTestCases
        {
            get
            {
                yield return new TestCaseData(3, "son")
                    .SetDescription("Verify filter by customer name containing 'son'");
            }
        }

        [SetUp]
        public async Task Setup()
        {
            _bookingsPage = new BookingsPage();
            await _bookingsPage.GoTo(Page);
        }

        [TestCaseSource(nameof(VerifyBookingsPageTestCases))]
        public async Task VerifyBookingsPage(int testCaseId)
        {
            await BasePage.VerifyPage<BookingsPageData>(Page, ExpectedResults, Results);
        }

        [TestCaseSource(nameof(VerifyBookingsFilterTestCases))]
        public async Task VerifyBookingsFilter(int testCaseId, string customerId)
        {
            await _bookingsPage.FilterBookings(Page, customerId);
            await BasePage.VerifyPage<BookingsPageData>(Page, ExpectedResults, Results);
        }

        [TestCaseSource(nameof(VerifyBookingsFilterByCustomerNameTestCases))]
        public async Task VerifyBookingsFilterByCustomerName(int testCaseId, string customerName)
        {
            await _bookingsPage.FilterBookingsByCustomerName(Page, customerName);
            await BasePage.VerifyPage<BookingsPageData>(Page, ExpectedResults, Results);
        }
    }
}