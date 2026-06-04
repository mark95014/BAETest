using LDSTest.Shared;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Newtonsoft.Json.Linq;

namespace LDSUITest.utils.PageData.Elements
{
    public class TableElement : Element
    {
        private readonly string _nextButtonSelector;
        private readonly string _firstButtonSelector;

        public TableElement(IWebDriver driver, By locator) : base(driver, locator)
        {
            _nextButtonSelector = "[id='btnNext']";
            _firstButtonSelector = "[id='btnFirst']";
        }

        private bool HasMultiplePages()
        {
            try
            {
                var nextButton = Driver.FindElement(By.CssSelector(_nextButtonSelector));
                return nextButton.Enabled;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public override void Get()
        {
            var gridData = new List<List<string>>();

            // Get headers if they exist (only once)
            var headers = GetHeaders();
            if (headers.Count > 0)
            {
                gridData.Add(headers);
            }

            if (HasMultiplePages())
            {
                // Navigate to first page
                GoToFirstPage();

                // Get all pages
                var allBodyRows = GetAllPaginatedRows();
                gridData.AddRange(allBodyRows);
            }
            else
            {
                // Get single page
                var bodyRows = GetBodyRows();
                gridData.AddRange(bodyRows);
            }

            Data = gridData;
        }

        private void GoToFirstPage()
        {
            try
            {
                var firstButton = Driver.FindElement(By.CssSelector(_firstButtonSelector));
                if (firstButton.Enabled)
                {
                    firstButton.Click();
                    Thread.Sleep(500); // Wait for table to reload
                }
            }
            catch (NoSuchElementException)
            {
                // First button doesn't exist, already on first page
            }
        }

        private List<List<string>> GetAllPaginatedRows()
        {
            var allRows = new List<List<string>>();

            while (true)
            {
                // Get rows from current page
                var pageRows = GetBodyRows();
                allRows.AddRange(pageRows);

                // Check if Next button exists and is enabled
                try
                {
                    var nextButton = Driver.FindElement(By.CssSelector(_nextButtonSelector));
                    if (!nextButton.Enabled)
                    {
                        break; // Next button disabled, we're on last page
                    }

                    // Click next and wait for table to reload
                    nextButton.Click();
                    Thread.Sleep(500); // Wait for page to load
                }
                catch (NoSuchElementException)
                {
                    break; // No next button, we're done
                }
            }

            return allRows;
        }

        public List<string> GetHeaders()
        {
            var headers = new List<string>();
            var table = Driver.FindElement(Locator);
            
            try
            {
                var headerCells = table.FindElements(By.CssSelector("thead th"));

                foreach (var cell in headerCells)
                {
                    headers.Add(cell.Text.Trim());
                }
            }
            catch (NoSuchElementException)
            {
                // No headers
            }

            return headers;
        }

        public List<List<string>> GetBodyRows()
        {
            var rowsData = new List<List<string>>();
            var table = Driver.FindElement(Locator);
            var rows = table.FindElements(By.CssSelector("tbody tr"));

            foreach (var row in rows)
            {
                var cells = row.FindElements(By.TagName("td"));
                var rowData = new List<string>();

                foreach (var cell in cells)
                {
                    rowData.Add(cell.Text.Trim());
                }

                rowsData.Add(rowData);
            }

            return rowsData;
        }

        public override Result Verify(string name, object expected)
        {
            var actualGrid = Data as List<List<string>>;
            var expectedGrid = ((JArray)expected).ToObject<List<List<string>>>();

            if (actualGrid!.Count != expectedGrid!.Count)
            {
                return new Result(false, $"{name}: row count mismatch. Expected {expectedGrid.Count}, actual {actualGrid.Count}");
            }

            for (int i = 0; i < expectedGrid.Count; i++)
            {
                if (actualGrid[i].Count != expectedGrid[i].Count)
                {
                    return new Result(false, $"{name}: column count mismatch in row {i}. Expected {expectedGrid[i].Count}, actual {actualGrid[i].Count}");
                }

                for (int j = 0; j < expectedGrid[i].Count; j++)
                {
                    if (actualGrid[i][j] != expectedGrid[i][j])
                    {
                        return new Result(false, $"{name}: cell mismatch at [{i},{j}]. Expected '{expectedGrid[i][j]}', actual '{actualGrid[i][j]}'");
                    }
                }
            }

            return new Result(true, $"{name}: grid data matches");
        }

        #region Utility Methods

        public int GetRowCount(bool includeHeader = false)
        {
            var table = Driver.FindElement(Locator);
            var rows = table.FindElements(By.CssSelector("tbody tr"));
            var bodyRowCount = rows.Count;

            if (includeHeader)
            {
                try
                {
                    var headers = table.FindElements(By.CssSelector("thead th"));
                    return headers.Count > 0 ? bodyRowCount + 1 : bodyRowCount;
                }
                catch (NoSuchElementException)
                {
                    return bodyRowCount;
                }
            }

            return bodyRowCount;
        }

        public int GetColumnCount()
        {
            var table = Driver.FindElement(Locator);
            
            // Try to get column count from headers first
            try
            {
                var headers = table.FindElements(By.CssSelector("thead th"));
                if (headers.Count > 0)
                {
                    return headers.Count;
                }
            }
            catch (NoSuchElementException) { }

            // Fallback to first body row
            var firstRow = table.FindElement(By.CssSelector("tbody tr"));
            var cells = firstRow.FindElements(By.TagName("td"));
            return cells.Count;
        }

        public string GetCellText(int row, int column)
        {
            var table = Driver.FindElement(Locator);
            var rows = table.FindElements(By.CssSelector("tbody tr"));
            var targetRow = rows[row];
            var cells = targetRow.FindElements(By.TagName("td"));
            return cells[column].Text.Trim();
        }

        public List<string> GetRowData(int rowIndex)
        {
            var table = Driver.FindElement(Locator);
            var rows = table.FindElements(By.CssSelector("tbody tr"));
            var targetRow = rows[rowIndex];
            var cells = targetRow.FindElements(By.TagName("td"));

            return cells.Select(cell => cell.Text.Trim()).ToList();
        }

        public List<string> GetColumnData(int columnIndex, bool includeHeader = false)
        {
            var columnData = new List<string>();
            var table = Driver.FindElement(Locator);

            // Add header if requested
            if (includeHeader)
            {
                try
                {
                    var headers = table.FindElements(By.CssSelector("thead th"));
                    if (columnIndex < headers.Count)
                    {
                        columnData.Add(headers[columnIndex].Text.Trim());
                    }
                }
                catch (NoSuchElementException) { }
            }

            // Add body cells
            var rows = table.FindElements(By.CssSelector("tbody tr"));
            foreach (var row in rows)
            {
                var cells = row.FindElements(By.TagName("td"));
                if (columnIndex < cells.Count)
                {
                    columnData.Add(cells[columnIndex].Text.Trim());
                }
            }

            return columnData;
        }

        public IWebElement GetRow(int index)
        {
            var table = Driver.FindElement(Locator);
            var rows = table.FindElements(By.CssSelector("tbody tr"));
            return rows[index];
        }

        public IWebElement GetCell(int row, int column)
        {
            var table = Driver.FindElement(Locator);
            var rows = table.FindElements(By.CssSelector("tbody tr"));
            var targetRow = rows[row];
            var cells = targetRow.FindElements(By.TagName("td"));
            return cells[column];
        }

        public Result VerifyRowCount(string name, int expectedCount, bool includeHeader = false)
        {
            var actualCount = GetRowCount(includeHeader);
            var message = $"{name}: row count={actualCount}, expected={expectedCount}";
            return new Result(actualCount == expectedCount, message);
        }

        public Result VerifyColumnCount(string name, int expectedCount)
        {
            var actualCount = GetColumnCount();
            var message = $"{name}: column count={actualCount}, expected={expectedCount}";
            return new Result(actualCount == expectedCount, message);
        }

        public Result VerifyCell(string name, int row, int column, string expectedValue)
        {
            var actualValue = GetCellText(row, column);
            var message = $"{name}: cell [{row},{column}]='{actualValue}', expected='{expectedValue}'";
            return new Result(actualValue == expectedValue, message);
        }

        #endregion
    }
}