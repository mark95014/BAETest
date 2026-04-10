using LDSUITest.src.utils.PageData;
using LDSUITest.src.utils.PageData.Elements;
using Microsoft.Playwright;

namespace LDSUITest.utils.PageData
{
    internal class CustomersPageData : BasePageData
    {
        public TextElement Title;
        public TableElement CustomersTable;

        protected override void InitializeElements()
        {
            Title = new TextElement(Page.GetByRole(AriaRole.Heading, new() { Name = "All Customers" }));
            CustomersTable = new TableElement(Page.Locator("[id='customersTable']"), supportsPagination: true);
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