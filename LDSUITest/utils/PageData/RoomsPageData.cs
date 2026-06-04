using LDSUITest.utils.PageData.Elements;
using OpenQA.Selenium;

namespace LDSUITest.utils.PageData
{
    public class RoomsPageData : BasePageData
    {
        public TextElement Title { get; set; } = null!;
        public TableElement RoomsTable { get; set; } = null!;

        // Constructor or initialization will set these values before use
        protected override void InitializeElements()
        {
            Title = new TextElement(Driver, By.CssSelector("body > h1"));
            RoomsTable = new TableElement(Driver, By.Id("roomsTable"));
        }
    }
}