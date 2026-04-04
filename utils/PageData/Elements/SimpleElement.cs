using Microsoft.Playwright;

namespace BAETest.src.utils.PageData.Elements
{
    public abstract class SimpleElement : Element
    {
        protected SimpleElement(ILocator locator) : base(locator)
        {
        }
    }
}