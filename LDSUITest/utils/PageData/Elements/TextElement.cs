using Microsoft.Playwright;
using LDSUITest.src.utils;

namespace LDSUITest.src.utils.PageData.Elements
{
    public class TextElement : Element
    {
        public string? Regex { get; set; }  // Make nullable

        public TextElement(ILocator locator) : base(locator)
        {
        }

        // Line 28 - Change constructor to accept nullable regex
        public TextElement(ILocator locator, string defaultValue = "", string? regex = null) 
            : base(locator)
        {
            Data = defaultValue;
            Regex = regex;
        }

        public override async Task GetAsync()
        {
            var textContent = await Locator.TextContentAsync();
            
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

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            await GetAsync();
            
            bool passed = Data?.ToString() == expected?.ToString();
            
            return new Result(passed, $"{name}: Expected '{expected}', Actual '{Data}'");
        }
    }
}