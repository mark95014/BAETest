using LDSTest.Shared;
using OpenQA.Selenium;

namespace LDSUITest.utils.PageData.Elements
{
    public class ValueElement : SimpleElement
    {
        private readonly string _attribute;

        public ValueElement(IWebDriver driver, By locator, string attribute = "value") : base(driver, locator)
        {
            _attribute = attribute;
        }

        public override void Get()
        {
            var element = Driver.FindElement(Locator);
            
            if (_attribute == "value")
            {
                Data = element.GetAttribute("value") ?? string.Empty;
            }
            else if (_attribute == "text" || _attribute == "textContent")
            {
                Data = element.Text ?? string.Empty;
            }
            else if (_attribute == "innerText")
            {
                Data = element.GetAttribute("innerText") ?? element.Text ?? string.Empty;
            }
            else
            {
                Data = element.GetAttribute(_attribute) ?? string.Empty;
            }
        }

        public override Result Verify(string name, object expected)
        {
            Get();
            string actualValue = Data?.ToString() ?? "";
            string expectedValue = expected?.ToString() ?? "";

            var message = $"{name}: {_attribute}='{actualValue}', expected='{expectedValue}'";
            return new Result(actualValue == expectedValue, message);
        }

        public Result VerifyAttribute(string name, string expectedValue)
        {
            try
            {
                Get();
                string actualValue = Data?.ToString() ?? "";
                bool passed = actualValue == expectedValue;

                if (passed)
                {
                    return new Result(true, $"{name}: {_attribute} matches '{expectedValue}'");
                }

                return new Result(false, $"{name}: expected '{expectedValue}', actual '{actualValue}'");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: error verifying attribute. {ex.Message}");
            }
        }
    }
}