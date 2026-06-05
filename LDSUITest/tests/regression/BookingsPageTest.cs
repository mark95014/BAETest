using LDSUITest.pages;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections;

namespace LDSUITest.tests.regression
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
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

        [Test]
        [TestCaseSource(nameof(BookingsPageTestCases))]
        public void VerifyBookingsPage(int testCaseId)
        {
            IWebDriver driver = CreateWebDriver(_browserType, _headless);
            new BookingsPage().GoTo(driver);
            BasePage.VerifyPage<BookingsPageData>(driver, ExpectedResults, Results);
            driver?.Quit();
        }

        [Test]
        [TestCaseSource(nameof(FilterByCustomerIdTestCases))]
        public void FilterByCustomerId(int testCaseId, string customerId)
        {
            IWebDriver driver = CreateWebDriver(_browserType, _headless);
            var bookingsPage = new BookingsPage();
            bookingsPage.GoTo(driver);
            bookingsPage.FilterBookingsByCustomerId(driver, customerId);
            BasePage.VerifyPage<BookingsPageData>(driver, ExpectedResults, Results);
            driver?.Quit();
        }

        [Test]
        [TestCaseSource(nameof(FilterByCustomerNameTestCases))]
        public void FilterByCustomerName(int testCaseId, string customerName)
        {
            IWebDriver driver = CreateWebDriver(_browserType, _headless);
            var bookingsPage = new BookingsPage();
            bookingsPage.GoTo(driver);
            bookingsPage.FilterBookingsByCustomerName(driver, customerName);
            BasePage.VerifyPage<BookingsPageData>(driver, ExpectedResults, Results);
            driver?.Quit();
        }
    }
}