using LDSTest.Shared;
using OpenQA.Selenium;

namespace LDSUITest.utils.PageData.Elements
{
    public class LegendElement : Element
    {
        public LegendElement(IWebDriver driver, By locator) : base(driver, locator)
        {
        }

        public override void Get()
        {
            // Get all legend items (typically a list of items with labels and colors)
            var legendContainer = Driver.FindElement(Locator);
            var items = legendContainer.FindElements(By.CssSelector(".legend-item, .legend > li, [class*='legend'] > *"));

            var legendData = new Dictionary<string, string>();

            foreach (var item in items)
            {
                var label = item.Text;

                // Get color/style (e.g., background-color, border-color)
                var colorIndicators = item.FindElements(By.CssSelector(".color, .swatch, [class*='indicator']"));
                string color = "";

                if (colorIndicators.Count > 0)
                {
                    color = colorIndicators[0].GetAttribute("style") ?? "";
                }

                legendData[label?.Trim() ?? string.Empty] = color;
            }

            Data = legendData;
        }

        public string GetLegendItemColor(string label)
        {
            var legendContainer = Driver.FindElement(Locator);
            var items = legendContainer.FindElements(By.CssSelector(".legend-item, .legend > li"));

            foreach (var item in items)
            {
                var itemLabel = item.Text;

                if (itemLabel?.Trim() == label)
                {
                    var colorIndicators = item.FindElements(By.CssSelector(".color, .swatch, [class*='indicator']"));
                    if (colorIndicators.Count > 0)
                    {
                        return colorIndicators[0].GetAttribute("style") ?? "";
                    }
                }
            }

            return string.Empty;
        }

        public override Result Verify(string name, object expected)
        {
            Get();

            if (expected is Dictionary<string, string> expectedLegend)
            {
                var actualLegend = Data as Dictionary<string, string>;

                if (actualLegend!.Count != expectedLegend.Count)
                {
                    return new Result(false, $"{name}: legend item count mismatch. Expected {expectedLegend.Count}, actual {actualLegend.Count}");
                }

                foreach (var kvp in expectedLegend)
                {
                    if (!actualLegend.ContainsKey(kvp.Key))
                    {
                        return new Result(false, $"{name}: legend item '{kvp.Key}' not found");
                    }

                    if (actualLegend[kvp.Key] != kvp.Value)
                    {
                        return new Result(false, $"{name}: legend item '{kvp.Key}' color mismatch. Expected '{kvp.Value}', actual '{actualLegend[kvp.Key]}'");
                    }
                }

                return new Result(true, $"{name}: legend matches");
            }

            return new Result(false, $"{name}: expected data is not in the correct format (Dictionary<string, string>)");
        }

        public Result VerifyLegendItemExists(string name, string label)
        {
            Get();
            var legendData = Data as Dictionary<string, string>;

            if (legendData!.ContainsKey(label))
            {
                return new Result(true, $"{name}: legend item '{label}' exists");
            }

            return new Result(false, $"{name}: legend item '{label}' not found");
        }
    }
}