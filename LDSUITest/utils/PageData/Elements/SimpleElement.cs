using OpenQA.Selenium;

namespace LDSUITest.utils.PageData.Elements
{
    public abstract class SimpleElement : Element
    {
        protected SimpleElement(IWebDriver driver, By locator) : base(driver, locator)
        {
        }
    }
}