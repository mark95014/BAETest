using LDSTest.Shared;
using LDSUITest.utils.PageData;
using OpenQA.Selenium;

namespace LDSUITest.pages
{
    internal class CustomersPage : BasePage
    {
        protected override string RelativePath => "/customers";

        internal static class Selectors
        {
            internal static By PageTitle = By.CssSelector("h1");
            internal static By CustomersTable = By.Id("customersTable");
            internal static By FilterIdInput = By.Id("filterId");
            internal static By FilterNameInput = By.Id("filterName");
            internal static By ApplyFiltersButton = By.XPath("//button[contains(text(), 'Apply Filters')]");
        }

        public override void WaitForPageToLoad(IWebDriver driver)
        {
            WaitForElement(driver, Selectors.PageTitle);
            WaitForElement(driver, Selectors.CustomersTable);
        }

        public void FilterCustomersById(IWebDriver driver, string filterValue)
        {
            var filterIdInput = driver.FindElement(Selectors.FilterIdInput);
            filterIdInput.Clear();
            filterIdInput.SendKeys(filterValue);

            var applyButton = driver.FindElement(Selectors.ApplyFiltersButton);
            applyButton.Click();

            WaitForPageToLoad(driver);
        }

        public void FilterCustomersByName(IWebDriver driver, string filterValue)
        {
            var filterNameInput = driver.FindElement(Selectors.FilterNameInput);
            filterNameInput.Clear();
            filterNameInput.SendKeys(filterValue);

            var applyButton = driver.FindElement(Selectors.ApplyFiltersButton);
            applyButton.Click();

            WaitForPageToLoad(driver);
        }

        public void VerifyPage(IWebDriver driver, ExpectedResults expectedResults, Results results)
        {
            BasePage.VerifyPage<CustomersPageData>(driver, expectedResults, results);
        }
    }
}