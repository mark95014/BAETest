using Microsoft.Playwright;

namespace LDSUITest.utils.PageData.Elements
{
    public class TextElement : Element
    {
        public string? Regex { get; set; }

        public TextElement(ILocator locator) : base(locator)
        {
        }

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