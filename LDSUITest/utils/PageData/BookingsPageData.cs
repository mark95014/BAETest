using LDSUITest.utils.PageData.Elements;
using OpenQA.Selenium;

namespace LDSUITest.utils.PageData
{
    public class BookingsPageData : BasePageData
    {
        public TextElement Title { get; set; } = null!;
        public TableElement BookingsTable { get; set; } = null!;

        protected override void InitializeElements()
        {
            Title = new TextElement(Driver, By.CssSelector("h1"));//body > h1
            BookingsTable = new TableElement(Driver, By.Id("bookingsTable"));
        }
    }
}