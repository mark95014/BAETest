using LDSTest.Shared;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace LDSUITest.utils.PageData.Elements
{
    public class ColumnHeaderElement : SimpleElement
    {
        public ColumnHeaderElement(IWebDriver driver, By locator) : base(driver, locator)
        {
        }

        public void Click()
        {
            Driver.FindElement(Locator).Click();
        }

        public void SortAscending()
        {
            Click();
            // Wait for sort indicator
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            wait.Until(d =>
            {
                var element = d.FindElement(Locator);
                var ascIndicators = element.FindElements(By.CssSelector(".tg-sort-asc, .sort-asc, [aria-sort='ascending']"));
                return ascIndicators.Count > 0;
            });
        }

        public void SortDescending()
        {
            // Click twice for descending (or check current state)
            var currentSort = GetSortDirection();

            if (currentSort == "none")
            {
                Click();
                Click();
            }
            else if (currentSort == "ascending")
            {
                Click();
            }

            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            wait.Until(d =>
            {
                var element = d.FindElement(Locator);
                var descIndicators = element.FindElements(By.CssSelector(".tg-sort-desc, .sort-desc, [aria-sort='descending']"));
                return descIndicators.Count > 0;
            });
        }

        public string GetSortDirection()
        {
            var element = Driver.FindElement(Locator);
            var ariaSort = element.GetAttribute("aria-sort");
            if (ariaSort != null) return ariaSort;

            var ascExists = element.FindElements(By.CssSelector(".tg-sort-asc, .sort-asc")).Count > 0;
            if (ascExists) return "ascending";

            var descExists = element.FindElements(By.CssSelector(".tg-sort-desc, .sort-desc")).Count > 0;
            if (descExists) return "descending";

            return "none";
        }

        public override void Get()
        {
            Data = Driver.FindElement(Locator).Text ?? string.Empty;
        }

        public override Result Verify(string name, object expected)
        {
            string actualText = Driver.FindElement(Locator).Text ?? string.Empty;
            string expectedText = expected?.ToString() ?? "";

            var message = $"{name}: column header='{actualText}', expected='{expectedText}'";
            return new Result(actualText?.Trim() == expectedText?.Trim(), message);
        }

        public Result VerifySortDirection(string name, string expectedDirection)
        {
            var actualDirection = GetSortDirection();
            var message = $"{name}: sort direction='{actualDirection}', expected='{expectedDirection}'";
            return new Result(actualDirection == expectedDirection, message);
        }
    }
}