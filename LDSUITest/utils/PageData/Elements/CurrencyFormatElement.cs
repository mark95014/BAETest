using OpenQA.Selenium;

namespace LDSUITest.utils.PageData.Elements
{
    public class CurrencyFormatElement : TextElement
    {
        public CurrencyFormatElement(IWebDriver driver, By locator)
            : base(driver, locator, "", @"(\$[\d,]*(\.\d{2})|—)")
        {
        }
    }
}