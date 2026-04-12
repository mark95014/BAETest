using Microsoft.Playwright; // Ensure this using is present for ILocator

namespace LDSUITest.utils.PageData.Elements
{
    // If you need to set TextModifiers, assign to a field or property as appropriate
    // Example: this._modifiers = new TextModifiers("(\\$[\\d,]*(\\.\\d{2})|—)");
    public class CurrencyFormatElement(ILocator locator) : TextElement(locator)
    {
    }
}