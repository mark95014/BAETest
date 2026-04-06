using LDSTest.src.utils.PageData;
using LDSTest.src.utils.PageData.Elements;
using Microsoft.Playwright;

namespace LDSTest.utils.PageData
{
    internal class RoomsPageData : BasePageData
    {
        public TextElement Title;
        public TableElement RoomsTable;

        protected override void InitializeElements()
        {
            Title = new TextElement(Page.GetByRole(AriaRole.Heading, new() { Name = "All Rooms" }));
            RoomsTable = new TableElement(Page.Locator("[id='roomsTable']"), supportsPagination: true);
        }

        public override System.Threading.Tasks.Task GetAsync()
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public override System.Threading.Tasks.Task VerifyAsync()
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}