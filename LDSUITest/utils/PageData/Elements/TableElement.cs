using Microsoft.Playwright;
using Newtonsoft.Json.Linq;

namespace LDSUITest.src.utils.PageData.Elements
{
    public class TableElement : Element
    {
        private readonly bool _supportsPagination;
        private readonly string _nextButtonSelector;
        private readonly string _firstButtonSelector;

        public TableElement(ILocator locator, bool supportsPagination = false) : base(locator)
        {
            _supportsPagination = supportsPagination;
            _nextButtonSelector = "[id='btnNext']";
            _firstButtonSelector = "[id='btnFirst']";
        }

        public override async Task GetAsync()
        {
            var gridData = new List<List<string>>();

            // Get headers if they exist (only once)
            var headers = await GetHeadersAsync();
            if (headers.Count > 0)
            {
                gridData.Add(headers);
            }

            if (_supportsPagination)
            {
                // Navigate to first page
                await GoToFirstPageAsync();

                // Get all pages
                var allBodyRows = await GetAllPaginatedRowsAsync();
                gridData.AddRange(allBodyRows);
            }
            else
            {
                // Get single page
                var bodyRows = await GetBodyRowsAsync();
                gridData.AddRange(bodyRows);
            }

            Data = gridData;
        }

        private async Task GoToFirstPageAsync()
        {
            var page = Locator.Page;
            var firstButton = page.Locator(_firstButtonSelector);

            // Check if first button exists and is enabled
            var count = await firstButton.CountAsync();
            if (count > 0)
            {
                var isDisabled = await firstButton.IsDisabledAsync();
                if (!isDisabled)
                {
                    await firstButton.ClickAsync();
                    // Wait for table to reload
                    await Task.Delay(500);
                }
            }
        }

        private async Task<List<List<string>>> GetAllPaginatedRowsAsync()
        {
            var allRows = new List<List<string>>();
            var page = Locator.Page;
            var nextButton = page.Locator(_nextButtonSelector);

            while (true)
            {
                // Get rows from current page
                var pageRows = await GetBodyRowsAsync();
                allRows.AddRange(pageRows);

                // Check if Next button exists and is enabled
                var nextCount = await nextButton.CountAsync();
                if (nextCount == 0)
                {
                    break; // No next button, we're done
                }

                var isDisabled = await nextButton.IsDisabledAsync();
                if (isDisabled)
                {
                    break; // Next button disabled, we're on last page
                }

                // Click next and wait for table to reload
                await nextButton.ClickAsync();
                await Task.Delay(500); // Wait for page to load
            }

            return allRows;
        }

        public async Task<List<string>> GetHeadersAsync()
        {
            var headers = new List<string>();
            var headerCells = Locator.Locator("thead th");
            var headerCount = await headerCells.CountAsync();

            for (int i = 0; i < headerCount; i++)
            {
                var cellText = await headerCells.Nth(i).TextContentAsync();
                headers.Add(cellText?.Trim() ?? "");
            }

            return headers;
        }

        public async Task<List<List<string>>> GetBodyRowsAsync()
        {
            var rowsData = new List<List<string>>();
            var rows = Locator.Locator("tbody tr");
            var rowCount = await rows.CountAsync();

            for (int i = 0; i < rowCount; i++)
            {
                var row = rows.Nth(i);
                var cells = row.Locator("td");
                var cellCount = await cells.CountAsync();

                var rowData = new List<string>();
                for (int j = 0; j < cellCount; j++)
                {
                    var cellText = await cells.Nth(j).TextContentAsync();
                    rowData.Add(cellText?.Trim() ?? "");
                }

                rowsData.Add(rowData);
            }

            return rowsData;
        }

        public async Task<int> GetRowCountAsync(bool includeHeader = false)
        {
            var rows = Locator.Locator("tbody tr");
            var bodyRowCount = await rows.CountAsync();

            if (includeHeader)
            {
                var headers = Locator.Locator("thead th");
                var hasHeaders = await headers.CountAsync() > 0;
                return hasHeaders ? bodyRowCount + 1 : bodyRowCount;
            }

            return bodyRowCount;
        }

        public async Task<int> GetColumnCountAsync()
        {
            // Try to get column count from headers first
            var headers = Locator.Locator("thead th");
            var headerCount = await headers.CountAsync();

            if (headerCount > 0)
            {
                return headerCount;
            }

            // Fallback to first body row
            var firstRow = Locator.Locator("tbody tr").Nth(0);
            var cells = firstRow.Locator("td");
            return await cells.CountAsync();
        }

        public async Task<string> GetCellTextAsync(int row, int column)
        {
            var rows = Locator.Locator("tbody tr");
            var targetRow = rows.Nth(row);
            var cells = targetRow.Locator("td");
            var cell = cells.Nth(column);
            return (await cell.TextContentAsync())?.Trim() ?? "";
        }

        public async Task<List<string>> GetRowDataAsync(int rowIndex)
        {
            var rows = Locator.Locator("tbody tr");
            var targetRow = rows.Nth(rowIndex);
            var cells = targetRow.Locator("td");
            var cellCount = await cells.CountAsync();

            var rowData = new List<string>();
            for (int i = 0; i < cellCount; i++)
            {
                var cellText = await cells.Nth(i).TextContentAsync();
                rowData.Add(cellText?.Trim() ?? "");
            }

            return rowData;
        }

        public async Task<List<string>> GetColumnDataAsync(int columnIndex, bool includeHeader = false)
        {
            var columnData = new List<string>();

            // Add header if requested
            if (includeHeader)
            {
                var headers = Locator.Locator("thead th");
                var headerCount = await headers.CountAsync();
                if (columnIndex < headerCount)
                {
                    var headerText = await headers.Nth(columnIndex).TextContentAsync();
                    columnData.Add(headerText?.Trim() ?? "");
                }
            }

            // Add body cells
            var rows = Locator.Locator("tbody tr");
            var rowCount = await rows.CountAsync();

            for (int i = 0; i < rowCount; i++)
            {
                var row = rows.Nth(i);
                var cells = row.Locator("td");
                var cellCount = await cells.CountAsync();

                if (columnIndex < cellCount)
                {
                    var cellText = await cells.Nth(columnIndex).TextContentAsync();
                    columnData.Add(cellText?.Trim() ?? "");
                }
            }

            return columnData;
        }

        public ILocator GetRow(int index)
        {
            var rows = Locator.Locator("tbody tr");
            return rows.Nth(index);
        }

        public ILocator GetCell(int row, int column)
        {
            var rows = Locator.Locator("tbody tr");
            var targetRow = rows.Nth(row);
            var cells = targetRow.Locator("td");
            return cells.Nth(column);
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            // Data already loaded by BasePageData.Get() - no need to call GetAsync() again
            var actualGrid = Data as List<List<string>>;
            var expectedGrid = ((JArray)expected).ToObject<List<List<string>>>();

            if (actualGrid.Count != expectedGrid.Count)
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

        public async Task<Result> VerifyRowCountAsync(string name, int expectedCount, bool includeHeader = false)
        {
            var actualCount = await GetRowCountAsync(includeHeader);
            var message = $"{name}: row count={actualCount}, expected={expectedCount}";
            return new Result(actualCount == expectedCount, message);
        }

        public async Task<Result> VerifyColumnCountAsync(string name, int expectedCount)
        {
            var actualCount = await GetColumnCountAsync();
            var message = $"{name}: column count={actualCount}, expected={expectedCount}";
            return new Result(actualCount == expectedCount, message);
        }

        public async Task<Result> VerifyCellAsync(string name, int row, int column, string expectedValue)
        {
            var actualValue = await GetCellTextAsync(row, column);
            var message = $"{name}: cell [{row},{column}]='{actualValue}', expected='{expectedValue}'";
            return new Result(actualValue == expectedValue, message);
        }
    }
}