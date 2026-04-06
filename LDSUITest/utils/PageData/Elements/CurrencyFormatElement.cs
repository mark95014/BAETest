using Microsoft.Playwright; // Ensure this using is present for ILocator

namespace LDSUITest.src.utils.PageData.Elements
{
    public class CurrencyFormatElement : TextElement
    {
        public CurrencyFormatElement(ILocator locator) : base(locator)
        {
            // If you need to set TextModifiers, assign to a field or property as appropriate
            // Example: this._modifiers = new TextModifiers("(\\$[\\d,]*(\\.\\d{2})|—)");
        }
    }
}