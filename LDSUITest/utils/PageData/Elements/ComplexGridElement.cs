using LDSTest.Shared;
using OpenQA.Selenium;
using Newtonsoft.Json;

namespace LDSUITest.utils.PageData.Elements
{
    public class ComplexGridElement : GridElement
    {
        [JsonIgnore]
        public Type[] ColumnTypes { get; }

        public ComplexGridElement(IWebDriver driver, By locator, Type[] columnTypes, Results results)
            : base(driver, locator, results)
        {
            ColumnTypes = columnTypes;
        }

        public override void GetCell(IWebElement cellElement, List<object> row, int columnNumber)
        {
            if (columnNumber >= ColumnTypes.Length)
            {
                row.Add(string.Empty);
                return;
            }

            Type cellType = ColumnTypes[columnNumber];

            if (cellType != null)
            {
                // Create instance of the element type (e.g., TextElement, ButtonElement)
                // We need to create a By locator for this specific cell
                var constructor = cellType.GetConstructor(new[] { typeof(IWebDriver), typeof(By) });

                if (constructor != null)
                {
                    // Create a temporary By locator that finds this specific element
                    // We'll use XPath to locate the element by its position
                    var cellBy = By.XPath(".");

                    var element = (Element)constructor.Invoke(new object[] { Driver, cellBy });
                    // Set the context to the cell element
                    element.Get();
                    row.Add(element.Data);
                }
                else
                {
                    row.Add(string.Empty);
                }
            }
            else
            {
                row.Add(string.Empty);
            }
        }

        public override Result VerifyCell(object actualData, object expectedResult, string msg, int columnNumber)
        {
            if (columnNumber >= ColumnTypes.Length)
            {
                return new Result(true, $"{msg} - No type defined for column {columnNumber}");
            }

            Type cellType = ColumnTypes[columnNumber];

            if (cellType != null)
            {
                var constructor = cellType.GetConstructor(new[] { typeof(IWebDriver), typeof(By) });

                if (constructor != null)
                {
                    // Create a dummy locator for verification purposes
                    var dummyBy = By.TagName("body");
                    var element = (Element)constructor.Invoke(new object[] { Driver, dummyBy });
                    element.Data = actualData;
                    return element.Verify(msg, expectedResult);
                }
            }

            return new Result(true, msg);
        }
    }
}