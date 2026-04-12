using LDSUITest.utils.PageData.Elements;

namespace LDSUITest.utils.PageData
{
    public class CustomersPageData : BasePageData
    {
        public TextElement Title { get; set; } = null!;
        public TableElement CustomersTable { get; set; } = null!;

        protected override void InitializeElements()
        {
            // Initialize elements with their locators
            Title = new TextElement(Page.Locator("selector-for-title"));
            CustomersTable = new TableElement(Page.Locator("selector-for-table"));
        }
    }
}