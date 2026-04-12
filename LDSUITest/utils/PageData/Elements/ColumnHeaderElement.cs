using Microsoft.Playwright;

namespace LDSUITest.utils.PageData.Elements
{
    public class ColumnHeaderElement(ILocator locator) : SimpleElement(locator)
    {
        public async Task ClickAsync()
        {
            await Locator.ClickAsync();
        }

        public async Task SortAscendingAsync()
        {
            await ClickAsync();
            // Wait for sort indicator
            var ascIndicator = Locator.Locator(".tg-sort-asc, .sort-asc, [aria-sort='ascending']");
            await ascIndicator.WaitForAsync();
        }

        public async Task SortDescendingAsync()
        {
            // Click twice for descending (or check current state)
            var currentSort = await GetSortDirectionAsync();

            if (currentSort == "none")
            {
                await ClickAsync();
                await ClickAsync();
            }
            else if (currentSort == "ascending")
            {
                await ClickAsync();
            }

            var descIndicator = Locator.Locator(".tg-sort-desc, .sort-desc, [aria-sort='descending']");
            await descIndicator.WaitForAsync();
        }

        public async Task<string> GetSortDirectionAsync()
        {
            var ariaSort = await Locator.GetAttributeAsync("aria-sort");
            if (ariaSort != null) return ariaSort;

            var ascExists = await Locator.Locator(".tg-sort-asc, .sort-asc").CountAsync() > 0;
            if (ascExists) return "ascending";

            var descExists = await Locator.Locator(".tg-sort-desc, .sort-desc").CountAsync() > 0;
            if (descExists) return "descending";

            return "none";
        }

        public override async Task GetAsync()
        {
            Data = await Locator.TextContentAsync() ?? string.Empty;
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            string actualText = await Locator.TextContentAsync() ?? string.Empty;
            string expectedText = expected?.ToString() ?? "";

            var message = $"{name}: column header='{actualText}', expected='{expectedText}'";
            return new Result(actualText?.Trim() == expectedText?.Trim(), message);
        }

        public async Task<Result> VerifySortDirectionAsync(string name, string expectedDirection)
        {
            var actualDirection = await GetSortDirectionAsync();
            var message = $"{name}: sort direction='{actualDirection}', expected='{expectedDirection}'";
            return new Result(actualDirection == expectedDirection, message);
        }
    }
}