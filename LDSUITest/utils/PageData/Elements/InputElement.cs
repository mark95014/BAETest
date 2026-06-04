using LDSTest.Shared;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace LDSUITest.utils.PageData.Elements
{
    public class InputElement : SimpleElement
    {
        public InputElement(IWebDriver driver, By locator) : base(driver, locator)
        {
        }

        public void Fill(string value)
        {
            var element = Driver.FindElement(Locator);
            element.Clear();
            element.SendKeys(value);
        }

        public void Clear()
        {
            Driver.FindElement(Locator).Clear();
        }

        public void Type(string text, int delayMs = 0)
        {
            // Type with delay between keystrokes (useful for testing autocomplete, etc.)
            var element = Driver.FindElement(Locator);
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                if (delayMs > 0)
                {
                    Thread.Sleep(delayMs);
                }
            }
        }

        public void Press(string key)
        {
            Driver.FindElement(Locator).SendKeys(key);
        }

        public void PressSequentially(string text)
        {
            var element = Driver.FindElement(Locator);
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
            }
        }

        public override void Get()
        {
            Data = Driver.FindElement(Locator).GetAttribute("value") ?? string.Empty;
        }

        public override Result Verify(string name, object expected)
        {
            string actualValue = Driver.FindElement(Locator).GetAttribute("value") ?? string.Empty;
            string expectedValue = expected?.ToString() ?? "";

            var message = $"{name}: value='{actualValue}', expected='{expectedValue}'";
            return new Result(actualValue == expectedValue, message);
        }

        public Result VerifyValue(string name, string expected)
        {
            try
            {
                var actual = Driver.FindElement(Locator).GetAttribute("value");
                bool passed = actual == expected;

                if (passed)
                {
                    return new Result(true, $"{name}: value matches '{expected}'");
                }

                return new Result(false, $"{name}: expected '{expected}', actual '{actual}'");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: error verifying value. {ex.Message}");
            }
        }

        public Result VerifyPlaceholder(string name, string expectedPlaceholder)
        {
            try
            {
                var actual = Driver.FindElement(Locator).GetAttribute("placeholder");
                bool passed = actual == expectedPlaceholder;

                if (passed)
                {
                    return new Result(true, $"{name}: placeholder matches '{expectedPlaceholder}'");
                }

                return new Result(false, $"{name}: expected '{expectedPlaceholder}', actual '{actual}'");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: error verifying placeholder. {ex.Message}");
            }
        }
    }
}