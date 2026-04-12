using Microsoft.Playwright;

namespace LDSUITest.utils.PageData.Elements
{
    public class SelectOptionElement : SimpleElement
    {
        public SelectOptionElement(ILocator locator) : base(locator)
        {
        }

        public async Task SelectByValueAsync(string value)
        {
            await Locator.SelectOptionAsync(value);
        }

        public async Task SelectByLabelAsync(string label)
        {
            await Locator.SelectOptionAsync(new SelectOptionValue { Label = label });
        }

        public async Task SelectByIndexAsync(int index)
        {
            await Locator.SelectOptionAsync(new SelectOptionValue { Index = index });
        }

        public override async Task GetAsync()
        {
            // Get the selected option's value
            Data = await Locator.InputValueAsync();
        }

        public async Task<string> GetSelectedTextAsync()
        {
            // Get the selected option's text (visible label)
            var selectedOption = Locator.Locator("option:checked");
            return await selectedOption.TextContentAsync() ?? string.Empty;
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            await GetAsync();
            string actualValue = Data?.ToString() ?? "";
            string expectedValue = expected?.ToString() ?? "";

            var message = $"{name}: selected value='{actualValue}', expected='{expectedValue}'";
            return new Result(actualValue == expectedValue, message);
        }

        public async Task<Result> VerifySelectedTextAsync(string name, string expectedText)
        {
            try
            {
                var selectedOption = Locator.Locator("option:checked");
                await Assertions.Expect(selectedOption).ToHaveTextAsync(expectedText);
                return new Result(true, $"{name}: selected text matches '{expectedText}'");
            }
            catch (Exception ex)
            {
                var actualText = await GetSelectedTextAsync();
                return new Result(false, $"{name}: expected '{expectedText}', actual '{actualText}'. {ex.Message}");
            }
        }
    }
}