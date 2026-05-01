using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;
using System.Collections;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]

    public class BookingsPageTest : BaseTest
    {
        private BookingsPage _bookingsPage = null!;

        // TestCase input
        private static IEnumerable BookingsPageTestCases
        {
            get { yield return new TestCaseData(1).SetDescription("Verify all data on Bookings page"); }
        }

        private static IEnumerable FilterByCustomerIdTestCases
        {
            get { yield return new TestCaseData(2, "7").SetDescription("Verify filter functionality on Bookings page"); }
        }

        private static IEnumerable FilterByCustomerNameTestCases
        {
            get { yield return new TestCaseData(3, "son").SetDescription("Verify filter by customer name containing 'son'"); }
        }

        [SetUp]
        public async Task Setup()
        {
            _bookingsPage = new BookingsPage();
            await _bookingsPage.GoTo(Page);
        }

        // DON'T REMOVE testCaseId parameter. It gets set in TestContext by NUnit when the TestCaseSource()'s are invoked.
        // It is retrieved from TestContext and used later by the ExpectedResults and Results classes.

        [TestCaseSource(nameof(BookingsPageTestCases))]
        public async Task VerifyBookingsPage(int testCaseId)
        {
            await BasePage.VerifyPage<BookingsPageData>(Page, ExpectedResults, Results);
        }

        [TestCaseSource(nameof(FilterByCustomerIdTestCases))]
        public async Task FilterByCustomerId(int testCaseId, string customerId)
        {
            Console.WriteLine($"Headless: {TestContext.Parameters["Playwright.LaunchOptions.Headless"]}");
            await _bookingsPage.FilterBookingsByCustomerId(Page, customerId);
            await BasePage.VerifyPage<BookingsPageData>(Page, ExpectedResults, Results);
        }

        [TestCaseSource(nameof(FilterByCustomerNameTestCases))]
        public async Task FilterByCustomerName(int testCaseId, string customerName)
        {
            await _bookingsPage.FilterBookingsByCustomerName(Page, customerName);
            await BasePage.VerifyPage<BookingsPageData>(Page, ExpectedResults, Results);
        }
    }
}