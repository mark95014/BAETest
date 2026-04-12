using LDSUITest.utils.PageData;
using LDSUITest.utils.PageData.Elements;
using Microsoft.Playwright;

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