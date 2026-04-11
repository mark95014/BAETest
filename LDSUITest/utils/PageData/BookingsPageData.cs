using LDSUITest.src.utils.PageData.Elements;
using LDSUITest.src.utils.PageData;

namespace LDSUITest.utils.PageData
{
    public class BookingsPageData : BasePageData
    {
        public TextElement Title { get; set; } = null!;
        public TableElement BookingsTable { get; set; } = null!;

        protected override void InitializeElements()
        {
            // Initialize elements with their locators
            Title = new TextElement(Page.Locator("selector-for-title"));
            BookingsTable = new TableElement(Page.Locator("selector-for-table"));
        }

        public override async Task GetAsync()
        {
            await Task.CompletedTask;
        }

        public override async Task VerifyAsync()
        {
            await Task.CompletedTask;
        }
    }
}