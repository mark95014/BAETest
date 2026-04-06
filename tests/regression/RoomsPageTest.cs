using LDSTest.pages;
using LDSTest.src.utils;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using RoomsData = LDSTest.utils.PageData.RoomsPageData;

namespace LDSTest.tests.regression
{
    [TestFixture]
    public class RoomsPageTest : BaseTest
    {
        private RoomsData _roomsPageData;

        [SetUp]
        public async Task Setup()
        {
            await Page.GotoAsync("https://localhost:7031/rooms");

            _roomsPageData = new RoomsData();
            _roomsPageData.Initialize(Page);
        }

        [TestCase(1, Description = "Verify all data on Rooms page")]
        public async Task VerifyRoomsPage(int testCaseId)
        {
            await RoomsPage.VerifyPage(Page);
        }
    }
}