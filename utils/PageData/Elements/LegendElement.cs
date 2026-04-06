using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LDSTest.src.utils.PageData.Elements
{
    public class LegendElement : Element
    {
        public LegendElement(ILocator locator) : base(locator)
        {
        }

        public override async Task GetAsync()
        {
            // Get all legend items (typically a list of items with labels and colors)
            var items = Locator.Locator(".legend-item, .legend > li, [class*='legend'] > *");
            var itemCount = await items.CountAsync();

            var legendData = new Dictionary<string, string>();

            for (int i = 0; i < itemCount; i++)
            {
                var item = items.Nth(i);
                var label = await item.TextContentAsync();
                
                // Get color/style (e.g., background-color, border-color)
                var colorIndicator = item.Locator(".color, .swatch, [class*='indicator']");
                string color = "";
                
                if (await colorIndicator.CountAsync() > 0)
                {
                    color = await colorIndicator.First.GetAttributeAsync("style") ?? "";
                }
                
                legendData[label?.Trim()] = color;
            }

            Data = legendData;
        }

        public async Task<string> GetLegendItemColorAsync(string label)
        {
            var items = Locator.Locator(".legend-item, .legend > li");
            var itemCount = await items.CountAsync();

            for (int i = 0; i < itemCount; i++)
            {
                var item = items.Nth(i);
                var itemLabel = await item.TextContentAsync();
                
                if (itemLabel?.Trim() == label)
                {
                    var colorIndicator = item.Locator(".color, .swatch, [class*='indicator']");
                    if (await colorIndicator.CountAsync() > 0)
                    {
                        return await colorIndicator.First.GetAttributeAsync("style") ?? "";
                    }
                }
            }

            return null;
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            await GetAsync();

            if (expected is Dictionary<string, string> expectedLegend)
            {
                var actualLegend = Data as Dictionary<string, string>;

                if (actualLegend.Count != expectedLegend.Count)
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

        public async Task<Result> VerifyLegendItemExistsAsync(string name, string label)
        {
            await GetAsync();
            var legendData = Data as Dictionary<string, string>;

            if (legendData.ContainsKey(label))
            {
                return new Result(true, $"{name}: legend item '{label}' exists");
            }
            else
            {
                return new Result(false, $"{name}: legend item '{label}' not found. Available items: {string.Join(", ", legendData.Keys)}");
            }
        }
    }
}