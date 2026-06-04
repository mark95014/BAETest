using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using LDSTest.Shared;
using LDSUITest.utils.PageData;
using LDSUITest.utils;
using TestContext = NUnit.Framework.TestContext;

namespace LDSUITest.pages
{
    public abstract class BasePage
    {
        private readonly string WebBaseUrl = TestContext.Parameters[$"{TestContext.Parameters["environment"]?.ToString()}.webBaseURL"] ?? "";

        // The relative path for this page (e.g., "/bookings", "/customers")
        protected abstract string RelativePath { get; }

        // Full URL constructed from base URL + relative path
        protected string Url => $"{WebBaseUrl}{RelativePath}";

        // Navigate to this page and wait for it to load (NO MORE ASYNC)
        public void GoTo(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(Url);
            WaitForPageToLoad(driver);
        }

        // Wait for page-specific elements to be ready (NO MORE ASYNC)
        // Each page must implement this to wait for its key elements
        public abstract void WaitForPageToLoad(IWebDriver driver);

        // Verify page data against expected results (NO MORE ASYNC)
        public static void VerifyPage<TPageData>(IWebDriver driver, ExpectedResults expectedResults, Results results) 
            where TPageData : BasePageData, new()
        {
            var pageData = new TPageData();
            pageData.Initialize(driver);
            var verifier = new CommonVerifyPage();
            verifier.Verify(pageData, expectedResults, results);
        }

        /// <summary>
        /// Wait for an element to be visible
        /// </summary>
        protected void WaitForElement(IWebDriver driver, By locator, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            wait.Until(d => d.FindElement(locator).Displayed);
        }

        /// <summary>
        /// Wait for an element to be clickable
        /// </summary>
        protected void WaitForElementClickable(IWebDriver driver, By locator, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            wait.Until(d =>
            {
                var element = d.FindElement(locator);
                return element.Displayed && element.Enabled;
            });
        }
    }
}