using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace BAETest.src.utils.PageData.Elements
{
    public class LinkElement : SimpleElement
    {
        public LinkElement(ILocator locator) : base(locator)
        {
        }

        public async Task ClickAsync()
        {
            await Locator.ClickAsync();
        }

        public async Task ClickAndWaitForNavigationAsync()
        {
            await Locator.ClickAsync();
            // Navigation is automatically waited for in Playwright
        }

        public async Task<string> GetHrefAsync()
        {
            return await Locator.GetAttributeAsync("href");
        }

        public override async Task GetAsync()
        {
            Data = await Locator.TextContentAsync();
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            string actualText = await Locator.TextContentAsync();
            string expectedText = expected?.ToString() ?? "";
            
            var message = $"{name}: text='{actualText}', expected='{expectedText}'";
            return new Result(actualText?.Trim() == expectedText?.Trim(), message);
        }

        public async Task<Result> VerifyHrefAsync(string name, string expectedHref)
        {
            try
            {
                await Assertions.Expect(Locator).ToHaveAttributeAsync("href", expectedHref);
                return new Result(true, $"{name}: href matches '{expectedHref}'");
            }
            catch (Exception ex)
            {
                var actual = await GetHrefAsync();
                return new Result(false, $"{name}: expected href '{expectedHref}', actual '{actual}'. {ex.Message}");
            }
        }
    }
}