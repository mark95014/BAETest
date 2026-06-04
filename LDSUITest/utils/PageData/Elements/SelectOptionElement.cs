using LDSTest.Shared;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace LDSUITest.utils.PageData.Elements
{
    public class SelectOptionElement : SimpleElement
    {
        public SelectOptionElement(IWebDriver driver, By locator) : base(driver, locator)
        {
        }

        public void SelectByValue(string value)
        {
            var selectElement = new SelectElement(Driver.FindElement(Locator));
            selectElement.SelectByValue(value);
        }

        public void SelectByLabel(string label)
        {
            var selectElement = new SelectElement(Driver.FindElement(Locator));
            selectElement.SelectByText(label);
        }

        public void SelectByIndex(int index)
        {
            var selectElement = new SelectElement(Driver.FindElement(Locator));
            selectElement.SelectByIndex(index);
        }

        public override void Get()
        {
            // Get the selected option's value
            var selectElement = new SelectElement(Driver.FindElement(Locator));
            Data = selectElement.SelectedOption.GetAttribute("value") ?? string.Empty;
        }

        public string GetSelectedText()
        {
            // Get the selected option's text (visible label)
            var selectElement = new SelectElement(Driver.FindElement(Locator));
            return selectElement.SelectedOption.Text ?? string.Empty;
        }

        public override Result Verify(string name, object expected)
        {
            Get();
            string actualValue = Data?.ToString() ?? "";
            string expectedValue = expected?.ToString() ?? "";

            var message = $"{name}: selected value='{actualValue}', expected='{expectedValue}'";
            return new Result(actualValue == expectedValue, message);
        }

        public Result VerifySelectedText(string name, string expectedText)
        {
            try
            {
                var actualText = GetSelectedText();
                bool passed = actualText == expectedText;

                if (passed)
                {
                    return new Result(true, $"{name}: selected text matches '{expectedText}'");
                }

                return new Result(false, $"{name}: expected '{expectedText}', actual '{actualText}'");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: error verifying selected text. {ex.Message}");
            }
        }
    }
}