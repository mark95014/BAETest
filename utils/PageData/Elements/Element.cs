using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace BAETest.src.utils.PageData.Elements
{
    public abstract class Element
    {
        protected ILocator Locator { get; }
        public object Data { get; set; }

        protected Element(ILocator locator)
        {
            Locator = locator ?? throw new ArgumentNullException(nameof(locator));
        }

        public abstract Task GetAsync();
        public abstract Task<Result> VerifyAsync(string name, object expected);
    }
}