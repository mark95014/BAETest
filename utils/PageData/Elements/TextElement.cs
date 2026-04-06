using Microsoft.Playwright;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LDSTest.src.utils.PageData.Elements
{
    public class TextElement : SimpleElement
    {
        private readonly TextModifiers _modifiers;

        public class TextModifiers
        {
            public int? Start { get; }
            public int? Length { get; }
            public string Regex { get; }

            public TextModifiers(int start, int length)
            {
                Start = start;
                Length = length;
            }

            public TextModifiers(string regex)
            {
                Regex = regex;
            }
        }

        public TextElement(ILocator locator, TextModifiers modifiers = null) 
            : base(locator)
        {
            _modifiers = modifiers;
        }

        public override async Task GetAsync()
        {
            string rawValue = await Locator.TextContentAsync();

            if (_modifiers?.Start != null && _modifiers.Length != null)
            {
                Data = rawValue.Substring((int)_modifiers.Start, (int)_modifiers.Length);
            }
            else
            {
                Data = rawValue;
            }
        }

        public override Task<Result> VerifyAsync(string name, object expected)
        {
            // Data already loaded by BasePageData.Get() - no need to call GetAsync() again
            var message = $"{name}: actual={Data} expected={expected}";

            if (_modifiers?.Regex != null)
            {
                string dataStr = Data?.ToString()?.Trim() ?? "";
                var regex = new Regex(_modifiers.Regex);
                return Task.FromResult(new Result(regex.IsMatch(dataStr), message));
            }
            else
            {
                bool matches = Data?.ToString() == expected?.ToString();
                return Task.FromResult(new Result(matches, message));
            }
        }

        // Direct Playwright assertions without Get
        public async Task<Result> VerifyTextAsync(string name, string expected)
        {
            try
            {
                await Assertions.Expect(Locator).ToHaveTextAsync(expected);
                return new Result(true, $"{name}: text matches '{expected}'");
            }
            catch (Exception ex)
            {
                var actual = await Locator.TextContentAsync();
                return new Result(false, $"{name}: expected '{expected}', actual '{actual}'. {ex.Message}");
            }
        }

        public async Task<Result> VerifyVisibleAsync(string name)
        {
            try
            {
                await Assertions.Expect(Locator).ToBeVisibleAsync();
                return new Result(true, $"{name}: element is visible");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: element not visible. {ex.Message}");
            }
        }
    }
}