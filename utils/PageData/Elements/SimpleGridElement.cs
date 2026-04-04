using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAETest.src.utils.PageData.Elements
{
    public class SimpleGridElement : Element
    {
        public SimpleGridElement(ILocator locator) : base(locator)
        {
        }

        public override async Task GetAsync()
        {
            // Get all rows
            var rows = Locator.Locator("tr, .row, [role='row']");
            var rowCount = await rows.CountAsync();
            
            var gridData = new List<List<string>>();

            for (int i = 0; i < rowCount; i++)
            {
                var row = rows.Nth(i);
                var cells = row.Locator("td, th, .cell, [role='cell']");
                var cellCount = await cells.CountAsync();
                
                var rowData = new List<string>();
                for (int j = 0; j < cellCount; j++)
                {
                    var cellText = await cells.Nth(j).TextContentAsync();
                    rowData.Add(cellText?.Trim() ?? "");
                }
                
                gridData.Add(rowData);
            }

            Data = gridData;
        }

        public async Task<int> GetRowCountAsync()
        {
            var rows = Locator.Locator("tr, .row, [role='row']");
            return await rows.CountAsync();
        }

        public async Task<string> GetCellTextAsync(int row, int column)
        {
            var rows = Locator.Locator("tr, .row, [role='row']");
            var targetRow = rows.Nth(row);
            var cells = targetRow.Locator("td, th, .cell, [role='cell']");
            var cell = cells.Nth(column);
            return await cell.TextContentAsync();
        }

        public ILocator GetRow(int index)
        {
            var rows = Locator.Locator("tr, .row, [role='row']");
            return rows.Nth(index);
        }

        public ILocator GetCell(int row, int column)
        {
            var rows = Locator.Locator("tr, .row, [role='row']");
            var targetRow = rows.Nth(row);
            var cells = targetRow.Locator("td, th, .cell, [role='cell']");
            return cells.Nth(column);
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            await GetAsync();
            
            if (expected is List<List<string>> expectedGrid)
            {
                var actualGrid = Data as List<List<string>>;
                
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

            return new Result(false, $"{name}: expected data is not in the correct format (List<List<string>>)");
        }

        public async Task<Result> VerifyRowCountAsync(string name, int expectedCount)
        {
            var actualCount = await GetRowCountAsync();
            var message = $"{name}: row count={actualCount}, expected={expectedCount}";
            return new Result(actualCount == expectedCount, message);
        }
    }
}