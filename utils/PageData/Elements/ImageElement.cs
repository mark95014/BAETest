using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace BAETest.src.utils.PageData.Elements
{
    public class ImageElement : SimpleElement
    {
        public ImageElement(ILocator locator) : base(locator)
        {
        }

        public async Task<string> GetSrcAsync()
        {
            return await Locator.GetAttributeAsync("src");
        }

        public async Task<string> GetAltAsync()
        {
            return await Locator.GetAttributeAsync("alt");
        }

        public override async Task GetAsync()
        {
            Data = await GetSrcAsync();
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            string actualSrc = await GetSrcAsync();
            string expectedSrc = expected?.ToString() ?? "";
            
            var message = $"{name}: src='{actualSrc}', expected='{expectedSrc}'";
            return new Result(actualSrc == expectedSrc, message);
        }

        public async Task<Result> VerifySrcAsync(string name, string expectedSrc)
        {
            try
            {
                await Assertions.Expect(Locator).ToHaveAttributeAsync("src", expectedSrc);
                return new Result(true, $"{name}: src matches '{expectedSrc}'");
            }
            catch (Exception ex)
            {
                var actual = await GetSrcAsync();
                return new Result(false, $"{name}: expected src '{expectedSrc}', actual '{actual}'. {ex.Message}");
            }
        }

        public async Task<Result> VerifyAltAsync(string name, string expectedAlt)
        {
            try
            {
                await Assertions.Expect(Locator).ToHaveAttributeAsync("alt", expectedAlt);
                return new Result(true, $"{name}: alt text matches '{expectedAlt}'");
            }
            catch (Exception ex)
            {
                var actual = await GetAltAsync();
                return new Result(false, $"{name}: expected alt '{expectedAlt}', actual '{actual}'. {ex.Message}");
            }
        }

        public async Task<Result> VerifyVisibleAsync(string name)
        {
            try
            {
                await Assertions.Expect(Locator).ToBeVisibleAsync();
                return new Result(true, $"{name}: image is visible");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: image not visible. {ex.Message}");
            }
        }
    }
}