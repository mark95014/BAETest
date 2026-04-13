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
            // Update these selectors to match your actual page
            Title = new TextElement(Page.Locator("h1:has-text('All Rooms')"));
            RoomsTable = new TableElement(Page.Locator("[id='roomsTable']"));
        }
    }
}