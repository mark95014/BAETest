using LDSTest.Shared;
using OpenQA.Selenium;
using Newtonsoft.Json;

namespace LDSUITest.utils.PageData.Elements
{
    public abstract class GridElement : Element
    {
        protected Results Results { get; }

        protected GridElement(IWebDriver driver, By locator, Results results) : base(driver, locator)
        {
            Results = results;
        }

        public abstract void GetCell(IWebElement cellElement, List<object> row, int columnNumber);
        public abstract Result VerifyCell(object actualResult, object expectedResult, string msg, int columnNumber);

        public override void Get()
        {
            GetGrid();
        }

        private void GetGrid()
        {
            var gridData = new List<List<object>>();
            var container = Driver.FindElement(Locator);

            // Check for pagination
            try
            {
                var pageCountElement = container.FindElement(By.CssSelector("*[id*=pagecount]"));
                var pageCountText = pageCountElement.Text;
                int pageCount = int.Parse(pageCountText);
                if (pageCount <= 0) pageCount = 1;

                var nextPageButton = container.FindElement(By.CssSelector("*[id*=page-next]"));

                for (int i = 0; i < pageCount; i++)
                {
                    GetPage(gridData, container);

                    if (i < pageCount - 1)
                    {
                        nextPageButton.Click();
                        Thread.Sleep(500);
                    }
                }
            }
            catch (NoSuchElementException)
            {
                // Single page grid
                GetPage(gridData, container);
            }

            Data = gridData;
        }

        private void GetPage(List<List<object>> gridData, IWebElement container)
        {
            Thread.Sleep(500); // Allow grid to render

            var rows = container.FindElements(By.CssSelector("div.tg-row, tr, [role='row']"));

            NUnit.Framework.TestContext.Progress.WriteLine($"GetPage: number of rows = {rows.Count}");

            foreach (var row in rows)
            {
                var cells = row.FindElements(By.CssSelector("div.tg-cell, td, [role='cell']"));

                NUnit.Framework.TestContext.Progress.WriteLine($"GetPage: number of cells = {cells.Count}");

                var rowData = new List<object>();

                for (int j = 0; j < cells.Count; j++)
                {
                    GetCell(cells[j], rowData, j);
                }

                gridData.Add(rowData);
            }
        }

        public int GetRowCount()
        {
            var container = Driver.FindElement(Locator);
            var rows = container.FindElements(By.CssSelector("div.tg-row, tr, [role='row']"));
            return rows.Count;
        }

        public IWebElement GetRow(int index)
        {
            var container = Driver.FindElement(Locator);
            var rows = container.FindElements(By.CssSelector("div.tg-row, tr, [role='row']"));
            return rows[index];
        }

        public IWebElement GetCell(int row, int column)
        {
            var container = Driver.FindElement(Locator);
            var rows = container.FindElements(By.CssSelector("div.tg-row, tr, [role='row']"));
            var targetRow = rows[row];
            var cells = targetRow.FindElements(By.CssSelector("div.tg-cell, td, [role='cell']"));
            return cells[column];
        }

        public override Result Verify(string name, object expected)
        {
            List<List<object>> expectedResult = JsonConvert.DeserializeObject<List<List<object>>>(expected.ToString()!)!;

            var actualData = Data as List<List<object>>;
            int actualRowCount = actualData?.Count ?? 0;
            int expectedRowCount = expectedResult?.Count ?? 0;

            if (actualRowCount != expectedRowCount)
            {
                string msg = $"Number of actual and expected rows are not the same. Name: {name} Actual: {actualRowCount} Expected: {expectedRowCount}";
                NUnit.Framework.TestContext.Progress.WriteLine(msg);
                return new Result(false, msg);
            }

            string message = $"Number of actual and expected rows are the same. Actual: {actualRowCount} Expected: {expectedRowCount}";
            NUnit.Framework.TestContext.Progress.WriteLine(message);

            if (expectedRowCount > 0)
            {
                for (int rowNum = 0; rowNum < expectedResult!.Count; rowNum++)
                {
                    var expectedRow = expectedResult[rowNum];
                    if (expectedRow != null)
                    {
                        for (int colNum = 0; colNum < expectedRow.Count; colNum++)
                        {
                            var expectedCell = expectedRow[colNum];
                            if (expectedCell != null)
                            {
                                if (colNum >= actualData![rowNum].Count || actualData[rowNum][colNum] == null)
                                {
                                    string msg = $"{name}: Missing actual result for column expected result: {expectedCell}";
                                    Results.Add(new Result(false, msg));
                                }
                                else
                                {
                                    string msg = $"{name}: actual= {actualData[rowNum][colNum]} expected= {expectedCell}";
                                    var result = VerifyCell(actualData[rowNum][colNum], expectedCell, msg, colNum);
                                    Results.Add(result);
                                }
                            }
                        }
                    }
                }
            }

            return new Result(true, "");
        }
    }
}
