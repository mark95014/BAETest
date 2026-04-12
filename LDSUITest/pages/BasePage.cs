using LDSUITest.utils;
using LDSUITest.utils.PageData;
using Microsoft.Playwright;

namespace LDSUITest.pages
{
    internal abstract class BasePage
    {
        /// <summary>
        /// The relative path for this page (e.g., "/bookings", "/customers")
        /// </summary>
        protected abstract string RelativePath { get; }

        /// <summary>
        /// Full URL constructed from base URL + relative path
        /// </summary>
        protected string Url => $"{BaseTest.WebBaseUrl}{RelativePath}";

        /// <summary>
        /// Navigate to this page and wait for it to load
        /// </summary>
        public async Task GoTo(IPage page)
        {
            await page.GotoAsync(Url);
            await WaitForPageToLoad(page);
        }

        /// <summary>
        /// Wait for page-specific elements to be ready.
        /// Each page must implement this to wait for its key elements.
        /// </summary>
        public abstract Task WaitForPageToLoad(IPage page);

        /// <summary>
        /// Verify page data against expected results.
        /// </summary>
        /// <typeparam name="TPageData">The PageData type to use for verification</typeparam>
        public static async Task VerifyPage<TPageData>(IPage page) where TPageData : BasePageData, new()
        {
            var pageData = new TPageData();
            pageData.Initialize(page);
            await CommonVerifyPage.Verify(pageData);
        }
    }
}