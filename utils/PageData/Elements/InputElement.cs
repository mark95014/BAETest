using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace LDSTest.src.utils.PageData.Elements
{
    public class InputElement : SimpleElement
    {
        public InputElement(ILocator locator) : base(locator)
        {
        }

        public async Task FillAsync(string value)
        {
            await Locator.FillAsync(value);
        }

        public async Task ClearAsync()
        {
            await Locator.ClearAsync();
        }

        public async Task TypeAsync(string text, int delayMs = 0)
        {
            // Type with delay between keystrokes (useful for testing autocomplete, etc.)
            foreach (char c in text)
            {
                await Locator.PressAsync(c.ToString());
                if (delayMs > 0)
                {
                    await Task.Delay(delayMs);
                }
            }
        }

        public async Task PressAsync(string key)
        {
            await Locator.PressAsync(key);
        }

        public async Task PressSequentiallyAsync(string text)
        {
            await Locator.PressSequentiallyAsync(text);
        }

        public override async Task GetAsync()
        {
            Data = await Locator.InputValueAsync();
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            string actualValue = await Locator.InputValueAsync();
            string expectedValue = expected?.ToString() ?? "";
            
            var message = $"{name}: value='{actualValue}', expected='{expectedValue}'";
            return new Result(actualValue == expectedValue, message);
        }

        public async Task<Result> VerifyValueAsync(string name, string expected)
        {
            try
            {
                await Assertions.Expect(Locator).ToHaveValueAsync(expected);
                return new Result(true, $"{name}: value matches '{expected}'");
            }
            catch (Exception ex)
            {
                var actual = await Locator.InputValueAsync();
                return new Result(false, $"{name}: expected '{expected}', actual '{actual}'. {ex.Message}");
            }
        }

        public async Task<Result> VerifyPlaceholderAsync(string name, string expectedPlaceholder)
        {
            try
            {
                await Assertions.Expect(Locator).ToHaveAttributeAsync("placeholder", expectedPlaceholder);
                return new Result(true, $"{name}: placeholder matches '{expectedPlaceholder}'");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: {ex.Message}");
            }
        }
    }
}