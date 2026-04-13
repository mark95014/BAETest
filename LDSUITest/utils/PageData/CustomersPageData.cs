using LDSUITest.utils.PageData.Elements;

namespace LDSUITest.utils.PageData
{
    public class CustomersPageData : BasePageData
    {
        public TextElement Title { get; set; } = null!;
        public TableElement CustomersTable { get; set; } = null!;

        protected override void InitializeElements()
        {
            // Update these selectors to match your actual page
            Title = new TextElement(Page.Locator("h1:has-text('All Customers')"));
            CustomersTable = new TableElement(Page.Locator("[id='customersTable']"));
        }
    }
}