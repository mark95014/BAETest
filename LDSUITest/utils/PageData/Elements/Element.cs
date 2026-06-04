using LDSTest.Shared;
using OpenQA.Selenium;

namespace LDSUITest.utils.PageData.Elements
{
    public abstract class Element
    {
        protected By Locator { get; }
        protected IWebDriver Driver { get; }
        public object Data { get; set; } = null!;

        protected Element(IWebDriver driver, By locator)
        {
            Driver = driver ?? throw new ArgumentNullException(nameof(driver));
            Locator = locator ?? throw new ArgumentNullException(nameof(locator));
        }

        public abstract void Get();
        public abstract Result Verify(string name, object expected);
    }
}