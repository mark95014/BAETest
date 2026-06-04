using LDSUITest.utils.PageData.Elements;
using OpenQA.Selenium;

namespace LDSUITest.utils.PageData
{
    public class CustomersPageData : BasePageData
    {
        public TextElement Title { get; set; } = null!;
        public TableElement CustomersTable { get; set; } = null!;

        protected override void InitializeElements()
        {
            // Update these selectors to match your actual page
            Title = new TextElement(Driver, By.CssSelector("body > h1"));
            CustomersTable = new TableElement(Driver, By.Id("customersTable"));
        }
    }
}