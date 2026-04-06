using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright; // Ensure this using is present for ILocator

namespace LDSTest.src.utils.PageData.Elements
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