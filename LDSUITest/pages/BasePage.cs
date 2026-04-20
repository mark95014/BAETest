using LDSTest.Shared;
using LDSUITest.utils;
using LDSUITest.utils.PageData;
using Microsoft.Playwright;
using NUnit.Framework;

namespace LDSUITest.pages
{
    public abstract class BasePage
    {
        private readonly string WebBaseUrl = TestContext.Parameters[$"{TestContext.Parameters["environment"]?.ToString()}.webBaseURL"] ?? "";

        // The relative path for this page (e.g., "/bookings", "/customers")
        protected abstract string RelativePath { get; }

        // Full URL constructed from base URL + relative path
        protected string Url => $"{WebBaseUrl}{RelativePath}";

        // Navigate to this page and wait for it to load
        public async Task GoTo(IPage page)
        {
            await page.GotoAsync(Url);
            await WaitForPageToLoad(page);
        }

        // Wait for page-specific elements to be ready.
        // Each page must implement this to wait for its key elements.
        public abstract Task WaitForPageToLoad(IPage page);

        // Verify page data against expected results.
        // <typeparam name="TPageData">The PageData type to use for verification</typeparam>
        public static async Task VerifyPage<TPageData>(IPage page, ExpectedResults expectedResults, Results results) where TPageData : BasePageData, new()
        {
            var pageData = new TPageData();
            pageData.Initialize(page);
            var verifier = new CommonVerifyPage();
            await verifier.Verify(pageData, expectedResults, results);
        }
    }
}