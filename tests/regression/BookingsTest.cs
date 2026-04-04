using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using BAETest.utils;

namespace BAETest.tests.regression;

[Parallelizable(ParallelScope.Self)]
public class BookingsTest : PageTest
{
    private BookingsData _pageData;

    [SetUp]
    public async Task Setup()
    {
        await Page.GotoAsync("https://localhost:7031/bookings");
        
        _pageData = new BookingsData();
        _pageData.Initialize(Page);
    }

    [TestCase(1)]
    public async Task VerifyBookingsPageDisplays(int testCaseId)
    {
        // Use your PageData pattern
        await _pageData.VerifyAsync();
    }

    [TestCase(2)]
    public async Task VerifyBookingsTitle(int testCaseId)
    {
        await _pageData.GetAsync();
        var result = await _pageData.Title.VerifyTextAsync("Title", "All Bookings");
        NUnit.Framework.Assert.That(result.passed, Is.True, result.message);
    }

    [TestCase(3)]
    public async Task VerifyHeadingVisible(int testCaseId)
    {
        // Mix PageData elements with direct Playwright assertions
        await Assertions.Expect(_pageData.Heading).ToBeVisibleAsync();
        await Assertions.Expect(_pageData.Heading).ToHaveTextAsync("All Bookings");
    }
}