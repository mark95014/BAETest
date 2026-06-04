using OpenQA.Selenium;
using LDSTest.Shared;
using LDSUITest.utils.PageData;

namespace LDSUITest.pages
{
    public class RoomsPage : BasePage
    {
        protected override string RelativePath => "/rooms";
        public class Room
        {
            public int RoomNumber { get; set; }
            public int Price { get; set; }
        }

        internal static class Selectors
        {
            internal static By PageTitle = By.CssSelector("body > h1')");
            internal static By RoomsTable = By.Id("roomsTable");
            internal static By NextPageButton = By.Id("btnNext");
        }

        public override void WaitForPageToLoad(IWebDriver driver)
        {
            //WaitForElement(driver, Selectors.PageTitle);
            WaitForElement(driver, Selectors.RoomsTable);
            WaitForElementClickable(driver, Selectors.NextPageButton);
            Thread.Sleep(500);
        }

        public void EditRoomPrices(IWebDriver driver, List<Room> rooms)
        {
            foreach (var room in rooms)
            {
                string roomNumber = room.RoomNumber.ToString();
                string newPrice = room.Price.ToString();

                // Find the row with the matching room number
                var row = driver.FindElement(By.XPath($"//table[@id='roomsTable']//tr[td[1][text()='{roomNumber}']]"));
                row.Click();

                // Edit the price
                var priceInput = driver.FindElement(By.Id("price"));
                priceInput.Clear();
                priceInput.SendKeys(newPrice);

                var saveButton = driver.FindElement(By.CssSelector(".btn-save"));
                saveButton.Click();

                // Wait for the table to reload
                WaitForPageToLoad(driver);
            }
        }

        public void VerifyPage(IWebDriver driver, ExpectedResults expectedResults, Results results)
        {
            BasePage.VerifyPage<RoomsPageData>(driver, expectedResults, results);
        }
    }
}