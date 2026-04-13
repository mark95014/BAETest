using LDSUITest.utils.PageData.Elements;

namespace LDSUITest.utils.PageData
{
    public class BookingsPageData : BasePageData
    {
        public TextElement Title { get; set; } = null!;
        public TableElement BookingsTable { get; set; } = null!;

        protected override void InitializeElements()
        {
            Title = new TextElement(Page.Locator("h1:has-text('All Bookings')"));
            BookingsTable = new TableElement(Page.Locator("[id='bookingsTable']"));
        }
    }
}