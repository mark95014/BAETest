using Microsoft.Playwright;

namespace LDSUITest.utils.PageData.Elements
{
    public class ValueElement : SimpleElement
    {
        private readonly string _attribute;

        public ValueElement(ILocator locator, string attribute = "value") : base(locator)
        {
            _attribute = attribute;
        }

        public override async Task GetAsync()
        {
            if (_attribute == "value")
            {
                Data = await Locator.InputValueAsync();
            }
            else if (_attribute == "text" || _attribute == "textContent")
            {
                Data = await Locator.TextContentAsync() ?? string.Empty;
            }
            else if (_attribute == "innerText")
            {
                Data = await Locator.InnerTextAsync();
            }
            else
            {
                Data = await Locator.GetAttributeAsync(_attribute) ?? string.Empty;
            }
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            await GetAsync();
            string actualValue = Data?.ToString() ?? "";
            string expectedValue = expected?.ToString() ?? "";

            var message = $"{name}: {_attribute}='{actualValue}', expected='{expectedValue}'";
            return new Result(actualValue == expectedValue, message);
        }

        public async Task<Result> VerifyAttributeAsync(string name, string expectedValue)
        {
            try
            {
                await Assertions.Expect(Locator).ToHaveAttributeAsync(_attribute, expectedValue);
                return new Result(true, $"{name}: {_attribute} matches '{expectedValue}'");
            }
            catch (Exception ex)
            {
                var actual = await Locator.GetAttributeAsync(_attribute);
                return new Result(false, $"{name}: expected '{expectedValue}', actual '{actual}'. {ex.Message}");
            }
        }
    }
}