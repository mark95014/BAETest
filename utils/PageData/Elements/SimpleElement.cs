using Microsoft.Playwright;

namespace LDSTest.src.utils.PageData.Elements
{
    public abstract class SimpleElement : Element
    {
        protected SimpleElement(ILocator locator) : base(locator)
        {
        }
    }
}