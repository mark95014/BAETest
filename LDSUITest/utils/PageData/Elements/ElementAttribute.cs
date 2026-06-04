using LDSTest.Shared;
using OpenQA.Selenium;

namespace LDSUITest.utils.PageData.Elements
{
    public class ElementAttribute : SimpleElement
    {
        private readonly string _attributeName;

        public ElementAttribute(IWebDriver driver, By locator, string attributeName) : base(driver, locator)
        {
            _attributeName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
        }

        public override void Get()
        {
            Data = Driver.FindElement(Locator).GetAttribute(_attributeName) ?? string.Empty;
        }

        public override Result Verify(string name, object expected)
        {
            Get();

            string actualValue = Data?.ToString() ?? "";
            string expectedValue = expected?.ToString() ?? "";

            var message = $"{name}: attribute '{_attributeName}'='{actualValue}', expected='{expectedValue}'";
            return new Result(actualValue == expectedValue, message);
        }

        public Result VerifyAttribute(string name, string expectedValue)
        {
            try
            {
                var actual = Driver.FindElement(Locator).GetAttribute(_attributeName);
                bool passed = actual == expectedValue;

                if (passed)
                {
                    return new Result(true, $"{name}: attribute '{_attributeName}' matches '{expectedValue}'");
                }

                return new Result(false, $"{name}: expected '{expectedValue}', actual '{actual}'");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: error verifying attribute. {ex.Message}");
            }
        }

        public Result VerifyAttributeContains(string name, string expectedSubstring)
        {
            Get();
            string actualValue = Data?.ToString() ?? "";
            bool contains = actualValue.Contains(expectedSubstring);

            var message = $"{name}: attribute '{_attributeName}'='{actualValue}' {(contains ? "contains" : "does not contain")} '{expectedSubstring}'";
            return new Result(contains, message);
        }
    }
}