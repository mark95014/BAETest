using LDSTest.Shared;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace LDSUITest.utils.PageData.Elements
{
    public class ButtonElement : SimpleElement
    {
        public ButtonElement(IWebDriver driver, By locator) : base(driver, locator)
        {
        }

        public void Click()
        {
            Driver.FindElement(Locator).Click();
        }

        public void DoubleClick()
        {
            var element = Driver.FindElement(Locator);
            var actions = new Actions(Driver);
            actions.DoubleClick(element).Perform();
        }

        public void Hover()
        {
            var element = Driver.FindElement(Locator);
            var actions = new Actions(Driver);
            actions.MoveToElement(element).Perform();
        }

        public override void Get()
        {
            // Get button state (enabled/disabled)
            Data = Driver.FindElement(Locator).Enabled;
        }

        public string GetText()
        {
            return Driver.FindElement(Locator).Text ?? string.Empty;
        }

        public override Result Verify(string name, object expected)
        {
            bool expectedEnabled = Convert.ToBoolean(expected);
            bool actualEnabled = Driver.FindElement(Locator).Enabled;

            var message = $"{name}: enabled={actualEnabled}, expected={expectedEnabled}";
            return new Result(actualEnabled == expectedEnabled, message);
        }

        public Result VerifyEnabled(string name, bool shouldBeEnabled = true)
        {
            try
            {
                bool actualEnabled = Driver.FindElement(Locator).Enabled;
                bool passed = actualEnabled == shouldBeEnabled;

                if (passed)
                {
                    return new Result(true, $"{name}: enabled state is correct");
                }

                return new Result(false, $"{name}: expected enabled={shouldBeEnabled}, actual={actualEnabled}");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: error checking enabled state. {ex.Message}");
            }
        }

        public Result VerifyText(string name, string expectedText)
        {
            try
            {
                var actualText = Driver.FindElement(Locator).Text;
                bool passed = actualText == expectedText;

                if (passed)
                {
                    return new Result(true, $"{name}: text matches '{expectedText}'");
                }

                return new Result(false, $"{name}: expected '{expectedText}', actual '{actualText}'");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: error verifying text. {ex.Message}");
            }
        }
    }
}