using Microsoft.Playwright;
using Newtonsoft.Json;

namespace LDSTest.src.utils.PageData.Elements
{
    public class ComplexGridElement : GridElement
    {
        [JsonIgnore]
        public Type[] ColumnTypes { get; }

        public ComplexGridElement(ILocator locator, Type[] columnTypes) : base(locator)
        {
            ColumnTypes = columnTypes;
        }

        public override async Task GetCellAsync(ILocator cellLocator, List<object> row, int columnNumber)
        {
            if (columnNumber >= ColumnTypes.Length)
            {
                row.Add(null);
                return;
            }

            Type cellType = ColumnTypes[columnNumber];

            if (cellType != null)
            {
                // Create instance of the element type (e.g., TextElement, ButtonElement)
                var constructor = cellType.GetConstructor(new[] { typeof(ILocator) });

                if (constructor != null)
                {
                    var element = (Element)constructor.Invoke(new object[] { cellLocator });
                    await element.GetAsync();
                    row.Add(element.Data);
                }
                else
                {
                    row.Add(null);
                }
            }
            else
            {
                row.Add(null);
            }
        }

        public override async Task<Result> VerifyCellAsync(object actualData, object expectedResult, string msg, int columnNumber)
        {
            if (columnNumber >= ColumnTypes.Length)
            {
                return new Result(true, $"{msg} - No type defined for column {columnNumber}");
            }

            Type cellType = ColumnTypes[columnNumber];

            if (cellType != null)
            {
                var constructor = cellType.GetConstructor(new[] { typeof(ILocator) });

                if (constructor != null)
                {
                    // Create a dummy locator for verification purposes
                    var dummyLocator = Locator.Page.Locator("body"); // Placeholder
                    var element = (Element)constructor.Invoke(new object[] { dummyLocator });
                    element.Data = actualData;
                    return await element.VerifyAsync(msg, expectedResult);
                }
            }

            return new Result(true, msg);
        }
    }
}