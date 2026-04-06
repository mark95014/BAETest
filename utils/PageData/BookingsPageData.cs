using LDSTest.src.utils.PageData;
using LDSTest.src.utils.PageData.Elements;
using Microsoft.Playwright;

namespace LDSTest.utils.PageData
{
    internal class BookingsPageData : BasePageData
    {
        public TextElement Title;
        public TableElement BookingsTable;

        protected override void InitializeElements()
        {
            Title = new TextElement(Page.GetByRole(AriaRole.Heading, new() { Name = "All Bookings" }));
            BookingsTable = new TableElement(Page.Locator("[id='bookingsTable']"), supportsPagination: true);
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