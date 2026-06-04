using LDSTest.Shared;
using OpenQA.Selenium;

namespace LDSUITest.utils.PageData.Elements
{
    public class ImageElement : SimpleElement
    {
        public ImageElement(IWebDriver driver, By locator) : base(driver, locator)
        {
        }

        public string GetSrc()
        {
            return Driver.FindElement(Locator).GetAttribute("src") ?? string.Empty;
        }

        public string GetAlt()
        {
            return Driver.FindElement(Locator).GetAttribute("alt") ?? string.Empty;
        }

        public override void Get()
        {
            Data = GetSrc();
        }

        public override Result Verify(string name, object expected)
        {
            string actualSrc = GetSrc();
            string expectedSrc = expected?.ToString() ?? "";

            var message = $"{name}: src='{actualSrc}', expected='{expectedSrc}'";
            return new Result(actualSrc == expectedSrc, message);
        }

        public Result VerifySrc(string name, string expectedSrc)
        {
            try
            {
                var actual = GetSrc();
                bool passed = actual == expectedSrc;

                if (passed)
                {
                    return new Result(true, $"{name}: src matches '{expectedSrc}'");
                }

                return new Result(false, $"{name}: expected src '{expectedSrc}', actual '{actual}'");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: error verifying src. {ex.Message}");
            }
        }

        public Result VerifyAlt(string name, string expectedAlt)
        {
            try
            {
                var actual = GetAlt();
                bool passed = actual == expectedAlt;

                if (passed)
                {
                    return new Result(true, $"{name}: alt text matches '{expectedAlt}'");
                }

                return new Result(false, $"{name}: expected alt '{expectedAlt}', actual '{actual}'");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: error verifying alt. {ex.Message}");
            }
        }

        public Result VerifyVisible(string name)
        {
            try
            {
                var element = Driver.FindElement(Locator);
                bool isVisible = element.Displayed;

                if (isVisible)
                {
                    return new Result(true, $"{name}: image is visible");
                }

                return new Result(false, $"{name}: image not visible");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: image not visible. {ex.Message}");
            }
        }
    }
}