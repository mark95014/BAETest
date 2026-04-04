using Microsoft.Playwright;
using Newtonsoft.Json;

namespace BAETest.src.utils.PageData.Elements
{
    public abstract class GridElement : Element
    {
        protected GridElement(ILocator locator) : base(locator)
        {
        }

        public abstract Task GetCellAsync(ILocator cellLocator, List<object> row, int columnNumber);
        public abstract Task<Result> VerifyCellAsync(object actualResult, object expectedResult, string msg, int columnNumber);

        public override async Task GetAsync()
        {
            await GetGridAsync();
        }

        private async Task GetGridAsync()
        {
            var gridData = new List<List<object>>();

            // Check for pagination
            var pageCountLocator = Locator.Locator("*[id*=pagecount]");
            var pageCountExists = await pageCountLocator.CountAsync() > 0;

            if (pageCountExists)
            {
                // Paginated grid
                var pageCountText = await pageCountLocator.TextContentAsync();
                int pageCount = int.Parse(pageCountText ?? "1");
                if (pageCount <= 0) pageCount = 1;

                var nextPageButton = Locator.Locator("*[id*=page-next]");

                for (int i = 0; i < pageCount; i++)
                {
                    await GetPageAsync(gridData);
                    
                    if (i < pageCount - 1)
                    {
                        await nextPageButton.ClickAsync();
                        await Task.Delay(500); // Wait for page to load
                    }
                }
            }
            else
            {
                // Single page grid
                await GetPageAsync(gridData);
            }

            Data = gridData;
        }

        private async Task GetPageAsync(List<List<object>> gridData)
        {
            await Task.Delay(500); // Allow grid to render

            var rows = Locator.Locator("div.tg-row, tr, [role='row']");
            var rowCount = await rows.CountAsync();

            NUnit.Framework.TestContext.Progress.WriteLine($"GetPageAsync: number of rows = {rowCount}");

            for (int i = 0; i < rowCount; i++)
            {
                var row = rows.Nth(i);
                var cells = row.Locator("div.tg-cell, td, [role='cell']");
                var cellCount = await cells.CountAsync();

                NUnit.Framework.TestContext.Progress.WriteLine($"GetPageAsync: number of cells = {cellCount}");

                var rowData = new List<object>();

                for (int j = 0; j < cellCount; j++)
                {
                    var cell = cells.Nth(j);
                    await GetCellAsync(cell, rowData, j);
                }

                gridData.Add(rowData);
            }
        }

        public async Task<int> GetRowCountAsync()
        {
            var rows = Locator.Locator("div.tg-row, tr, [role='row']");
            return await rows.CountAsync();
        }

        public ILocator GetRow(int index)
        {
            var rows = Locator.Locator("div.tg-row, tr, [role='row']");
            return rows.Nth(index);
        }

        public ILocator GetCell(int row, int column)
        {
            var rows = Locator.Locator("div.tg-row, tr, [role='row']");
            var targetRow = rows.Nth(row);
            var cells = targetRow.Locator("div.tg-cell, td, [role='cell']");
            return cells.Nth(column);
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            List<List<object>> expectedResult = JsonConvert.DeserializeObject<List<List<object>>>(expected.ToString());

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
                for (int rowNum = 0; rowNum < expectedResult.Count; rowNum++)
                {
                    var expectedRow = expectedResult[rowNum];
                    if (expectedRow != null)
                    {
                        for (int colNum = 0; colNum < expectedRow.Count; colNum++)
                        {
                            var expectedCell = expectedRow[colNum];
                            if (expectedCell != null)
                            {
                                if (colNum >= actualData[rowNum].Count || actualData[rowNum][colNum] == null)
                                {
                                    string msg = $"{name}: Missing actual result for column expected result: {expectedCell}";
                                    Test.results.Add(new Result(false, msg));
                                }
                                else
                                {
                                    string msg = $"{name}: actual= {actualData[rowNum][colNum]} expected= {expectedCell}";
                                    var result = await VerifyCellAsync(actualData[rowNum][colNum], expectedCell, msg, colNum);
                                    Test.results.Add(result);
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
