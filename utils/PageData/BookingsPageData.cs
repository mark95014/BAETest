using BAETest.src.utils.PageData;
using BAETest.src.utils.PageData.Elements;
using Microsoft.Playwright;

namespace BAETest.utils.PageData
{
    internal class BookingsPageData : BasePageData
    {
        internal TextElement Title;
        internal SimpleGridElement BookingsTable;

        protected override void InitializeElements()
        {
            Title = new TextElement(Page.GetByRole(AriaRole.Heading, new() { Name = "All Bookings" }));
            BookingsTable = new SimpleGridElement(Page.Locator("[data-testid='bookingsTable']"));
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