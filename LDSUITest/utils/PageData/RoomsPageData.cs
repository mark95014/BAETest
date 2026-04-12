using LDSUITest.utils.PageData.Elements;

namespace LDSUITest.utils.PageData
{
    public class RoomsPageData : BasePageData
    {
        public TextElement Title { get; set; } = null!;
        public TableElement RoomsTable { get; set; } = null!;

        // Constructor or initialization will set these values before use
        protected override void InitializeElements()
        {
            // Initialize elements with their locators
            Title = new TextElement(Page.Locator("selector-for-title"));
            RoomsTable = new TableElement(Page.Locator("selector-for-table"));
        }
    }
}