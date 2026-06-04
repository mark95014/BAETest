using LDSTest.Shared;
using OpenQA.Selenium;

namespace LDSUITest.utils.PageData.Elements
{
    public class CheckboxElement : SimpleElement
    {
        public CheckboxElement(IWebDriver driver, By locator) : base(driver, locator)
        {
        }

        public void Check()
        {
            var element = Driver.FindElement(Locator);
            if (!element.Selected)
            {
                element.Click();
            }
        }

        public void Uncheck()
        {
            var element = Driver.FindElement(Locator);
            if (element.Selected)
            {
                element.Click();
            }
        }

        public void SetChecked(bool shouldBeChecked)
        {
            if (shouldBeChecked)
            {
                Check();
            }
            else
            {
                Uncheck();
            }
        }

        public void Toggle()
        {
            Driver.FindElement(Locator).Click();
        }

        public override void Get()
        {
            Data = Driver.FindElement(Locator).Selected;
        }

        public override Result Verify(string name, object expected)
        {
            bool expectedChecked = Convert.ToBoolean(expected);
            bool actualChecked = Driver.FindElement(Locator).Selected;

            var message = $"{name}: checked={actualChecked}, expected={expectedChecked}";
            return new Result(actualChecked == expectedChecked, message);
        }

        public Result VerifyChecked(string name, bool shouldBeChecked = true)
        {
            try
            {
                bool actualChecked = Driver.FindElement(Locator).Selected;
                bool passed = actualChecked == shouldBeChecked;

                if (passed)
                {
                    return new Result(true, $"{name}: checked state is correct");
                }

                return new Result(false, $"{name}: expected checked={shouldBeChecked}, actual={actualChecked}");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: error checking checkbox state. {ex.Message}");
            }
        }
    }
}