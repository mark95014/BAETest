using LDSTest.Shared;
using Microsoft.Playwright;

namespace LDSUITest.utils.PageData.Elements
{
    public class CheckboxElement(ILocator locator) : SimpleElement(locator)
    {
        public async Task CheckAsync()
        {
            await Locator.CheckAsync();
        }

        public async Task UncheckAsync()
        {
            await Locator.UncheckAsync();
        }

        public async Task SetCheckedAsync(bool shouldBeChecked)
        {
            await Locator.SetCheckedAsync(shouldBeChecked);
        }

        public async Task ToggleAsync()
        {
            bool isChecked = await Locator.IsCheckedAsync();
            await SetCheckedAsync(!isChecked);
        }

        public override async Task GetAsync()
        {
            Data = await Locator.IsCheckedAsync();
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            bool expectedChecked = Convert.ToBoolean(expected);
            bool actualChecked = await Locator.IsCheckedAsync();

            var message = $"{name}: checked={actualChecked}, expected={expectedChecked}";
            return new Result(actualChecked == expectedChecked, message);
        }

        public async Task<Result> VerifyCheckedAsync(string name, bool shouldBeChecked = true)
        {
            try
            {
                if (shouldBeChecked)
                    await Assertions.Expect(Locator).ToBeCheckedAsync();
                else
                    await Assertions.Expect(Locator).Not.ToBeCheckedAsync();

                return new Result(true, $"{name}: checked state is correct");
            }
            catch (Exception ex)
            {
                var actual = await Locator.IsCheckedAsync();
                return new Result(false, $"{name}: expected checked={shouldBeChecked}, actual={actual}. {ex.Message}");
            }
        }
    }
}