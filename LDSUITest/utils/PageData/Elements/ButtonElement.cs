using Microsoft.Playwright;

namespace LDSUITest.utils.PageData.Elements
{
    public class ButtonElement(ILocator locator) : SimpleElement(locator)
    {
        public async Task ClickAsync()
        {
            await Locator.ClickAsync();
        }

        public async Task DoubleClickAsync()
        {
            await Locator.DblClickAsync();
        }

        public async Task HoverAsync()
        {
            await Locator.HoverAsync();
        }

        public override async Task GetAsync()
        {
            // Get button state (enabled/disabled)
            Data = await Locator.IsEnabledAsync();
        }

        public async Task<string> GetTextAsync()
        {
            return await Locator.TextContentAsync() ?? string.Empty;
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            bool expectedEnabled = Convert.ToBoolean(expected);
            bool actualEnabled = await Locator.IsEnabledAsync();

            var message = $"{name}: enabled={actualEnabled}, expected={expectedEnabled}";
            return new Result(actualEnabled == expectedEnabled, message);
        }

        public async Task<Result> VerifyEnabledAsync(string name, bool shouldBeEnabled = true)
        {
            try
            {
                if (shouldBeEnabled)
                    await Assertions.Expect(Locator).ToBeEnabledAsync();
                else
                    await Assertions.Expect(Locator).ToBeDisabledAsync();

                return new Result(true, $"{name}: enabled state is correct");
            }
            catch (Exception ex)
            {
                var actual = await Locator.IsEnabledAsync();
                return new Result(false, $"{name}: expected enabled={shouldBeEnabled}, actual={actual}. {ex.Message}");
            }
        }

        public async Task<Result> VerifyTextAsync(string name, string expectedText)
        {
            try
            {
                await Assertions.Expect(Locator).ToHaveTextAsync(expectedText);
                return new Result(true, $"{name}: text matches '{expectedText}'");
            }
            catch (Exception ex)
            {
                var actual = await Locator.TextContentAsync();
                return new Result(false, $"{name}: expected '{expectedText}', actual '{actual}'. {ex.Message}");
            }
        }
    }
}