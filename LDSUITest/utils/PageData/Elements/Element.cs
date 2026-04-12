using Microsoft.Playwright;

namespace LDSUITest.utils.PageData.Elements
{
    public abstract class Element
    {
        protected ILocator Locator { get; }
        public object Data { get; set; } = null!;

        protected Element(ILocator locator)
        {
            Locator = locator ?? throw new ArgumentNullException(nameof(locator));
        }

        public abstract Task GetAsync();
        public abstract Task<Result> VerifyAsync(string name, object expected);
    }
}