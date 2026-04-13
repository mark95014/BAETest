using LDSTest.Shared;
using Microsoft.Playwright;

namespace LDSUITest.utils.PageData.Elements
{
    public class ElementAttribute : SimpleElement
    {
        private readonly string _attributeName;

        public ElementAttribute(ILocator locator, string attributeName) : base(locator)
        {
            _attributeName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
        }

        public override async Task GetAsync()
        {
            Data = await Locator.GetAttributeAsync(_attributeName) ?? string.Empty;
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            await GetAsync();

            string actualValue = Data?.ToString() ?? "";
            string expectedValue = expected?.ToString() ?? "";

            var message = $"{name}: attribute '{_attributeName}'='{actualValue}', expected='{expectedValue}'";
            return new Result(actualValue == expectedValue, message);
        }

        public async Task<Result> VerifyAttributeAsync(string name, string expectedValue)
        {
            try
            {
                await Assertions.Expect(Locator).ToHaveAttributeAsync(_attributeName, expectedValue);
                return new Result(true, $"{name}: attribute '{_attributeName}' matches '{expectedValue}'");
            }
            catch (Exception ex)
            {
                var actual = await Locator.GetAttributeAsync(_attributeName);
                return new Result(false, $"{name}: expected '{expectedValue}', actual '{actual}'. {ex.Message}");
            }
        }

        public async Task<Result> VerifyAttributeContainsAsync(string name, string expectedSubstring)
        {
            await GetAsync();
            string actualValue = Data?.ToString() ?? "";
            bool contains = actualValue.Contains(expectedSubstring);

            var message = $"{name}: attribute '{_attributeName}'='{actualValue}' {(contains ? "contains" : "does not contain")} '{expectedSubstring}'";
            return new Result(contains, message);
        }
    }
}