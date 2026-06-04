using LDSTest.Shared;
using OpenQA.Selenium;

namespace LDSUITest.utils.PageData.Elements
{
    public class TextElement : Element
    {
        public string? Regex { get; set; }

        public TextElement(IWebDriver driver, By locator) : base(driver, locator)
        {
        }

        public TextElement(IWebDriver driver, By locator, string defaultValue = "", string? regex = null) 
            : base(driver, locator)
        {
            Data = defaultValue;
            Regex = regex;
        }

        public override void Get()
        {
            var element = Driver.FindElement(Locator);
            var textContent = element.Text;
            
            if (!string.IsNullOrEmpty(Regex) && textContent != null)
            {
                var match = System.Text.RegularExpressions.Regex.Match(textContent, Regex);
                Data = match.Success ? match.Value : textContent;
            }
            else
            {
                Data = textContent ?? string.Empty;
            }
        }

        public override Result Verify(string name, object expected)
        {            
            bool passed = Data?.ToString() == expected?.ToString();
            
            return new Result(passed, $"{name}: Expected '{expected}', Actual '{Data}'");
        }
    }
}