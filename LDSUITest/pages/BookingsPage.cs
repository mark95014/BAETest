using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using LDSTest.Shared;
using LDSUITest.utils;
using LDSUITest.utils.PageData;

namespace LDSUITest.pages
{
    internal class BookingsPage : BasePage
    {
        protected override string RelativePath => "/bookings";

        internal static class Selectors
        {
            internal static By PageTitle = By.CssSelector("body > h1");
            internal static By BookingsTable = By.Id("bookingsTable");
            internal static By CustomerIdFilter = By.Id("filterCustomerId");
            internal static By CustomerNameFilter = By.Id("filterCustomerName");
            internal static By ApplyFiltersButton = By.CssSelector("button[onclick='applyFilters()']");//body > div.filters > button:nth-child(5)
        }

        public override void WaitForPageToLoad(IWebDriver driver)
        {
            WaitForElement(driver, Selectors.PageTitle);
            WaitForElement(driver, Selectors.BookingsTable);
            
            // Additional wait for dynamic content
            Thread.Sleep(500);
        }

        public void FilterBookingsByCustomerId(IWebDriver driver, string customerId)
        {
            var customerIdInput = driver.FindElement(Selectors.CustomerIdFilter);
            customerIdInput.Clear();
            customerIdInput.SendKeys(customerId);

            var applyButton = driver.FindElement(Selectors.ApplyFiltersButton);
            applyButton.Click();

            WaitForPageToLoad(driver);
        }

        public void FilterBookingsByCustomerName(IWebDriver driver, string customerName)
        {
            var customerNameInput = driver.FindElement(Selectors.CustomerNameFilter);
            customerNameInput.Clear();
            customerNameInput.SendKeys(customerName);

            var applyButton = driver.FindElement(Selectors.ApplyFiltersButton);
            applyButton.Click();

            WaitForPageToLoad(driver);
        }

        public void VerifyPage(IWebDriver driver, ExpectedResults expectedResults, Results results)
        {
            BasePage.VerifyPage<BookingsPageData>(driver, expectedResults, results);
        }
    }
}