using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;
using System.Collections;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    public class BookingsPageTest : BaseTest
    {
        private static IEnumerable BookingsPageTestCases
        {
            get
            {
                yield return new TestCaseData(1)
                    .SetDescription("Verify all data on Bookings page");
            }
        }

        private static IEnumerable FilterByCustomerIdTestCases
        {
            get
            {
                yield return new TestCaseData(2, "7")
                    .SetDescription("Verify filter functionality on Bookings page");
            }
        }

        private static IEnumerable FilterByCustomerNameTestCases
        {
            get
            {
                yield return new TestCaseData(3, "son")
                    .SetDescription("Verify filter by customer name containing 'son'");
            }
        }

        private BookingsPage NavigateToBookingsPage()
        {
            var bookingsPage = new BookingsPage();
            bookingsPage.GoTo(Driver);  // No await!
            return bookingsPage;
        }

        [TestCaseSource(nameof(BookingsPageTestCases))]
        public void VerifyBookingsPage(int testCaseId)  // No async!
        {
            NavigateToBookingsPage();
            BasePage.VerifyPage<BookingsPageData>(Driver, ExpectedResults, Results);
        }

        [TestCaseSource(nameof(FilterByCustomerIdTestCases))]
        public void FilterByCustomerId(int testCaseId, string customerId)  // No async!
        {
            var bookingsPage = NavigateToBookingsPage();
            bookingsPage.FilterBookingsByCustomerId(Driver, customerId);
            BasePage.VerifyPage<BookingsPageData>(Driver, ExpectedResults, Results);
        }

        [TestCaseSource(nameof(FilterByCustomerNameTestCases))]
        public void FilterByCustomerName(int testCaseId, string customerName)  // No async!
        {
            var bookingsPage = NavigateToBookingsPage();
            bookingsPage.FilterBookingsByCustomerName(Driver, customerName);
            BasePage.VerifyPage<BookingsPageData>(Driver, ExpectedResults, Results);
        }
    }
}