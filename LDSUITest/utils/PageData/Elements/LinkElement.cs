using LDSTest.Shared;
using OpenQA.Selenium;

namespace LDSUITest.utils.PageData.Elements
{
    public class LinkElement : SimpleElement
    {
        public LinkElement(IWebDriver driver, By locator) : base(driver, locator)
        {
        }

        public void Click()
        {
            Driver.FindElement(Locator).Click();
        }

        public void ClickAndWaitForNavigation()
        {
            Driver.FindElement(Locator).Click();
            // In Selenium, you might want to add explicit wait logic here if needed
        }

        public string GetHref()
        {
            return Driver.FindElement(Locator).GetAttribute("href") ?? string.Empty;
        }

        public override void Get()
        {
            Data = Driver.FindElement(Locator).Text ?? string.Empty;
        }

        public override Result Verify(string name, object expected)
        {
            string actualText = Driver.FindElement(Locator).Text ?? string.Empty;
            string expectedText = expected?.ToString() ?? "";

            var message = $"{name}: text='{actualText}', expected='{expectedText}'";
            return new Result(actualText?.Trim() == expectedText?.Trim(), message);
        }

        public Result VerifyHref(string name, string expectedHref)
        {
            try
            {
                var actual = GetHref();
                bool passed = actual == expectedHref;

                if (passed)
                {
                    return new Result(true, $"{name}: href matches '{expectedHref}'");
                }

                return new Result(false, $"{name}: expected href '{expectedHref}', actual '{actual}'");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: error verifying href. {ex.Message}");
            }
        }
    }
}