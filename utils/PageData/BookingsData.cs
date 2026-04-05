using BAETest.src.utils;
using BAETest.src.utils.PageData;
using BAETest.src.utils.PageData.Elements;
using Microsoft.Playwright;

namespace BAETest.utils
{
    public class BookingsData : PageData
    {
        // Playwright elements
        public TextElement Title { get; private set; }
        public TextElement HeadingAllBookings { get; private set; }
        
        // Or use ILocator directly for simple cases
        public ILocator Heading { get; private set; }
        public ILocator NewBookingButton { get; private set; }
        public ILocator BookingsGrid { get; private set; }

        protected override void InitializeElements()
        {
            // Using TextElement wrapper
            Title = new TextElement(
                Page.GetByRole(AriaRole.Heading, new() { Name = "All Bookings" })
            );
            
            HeadingAllBookings = new TextElement(
                Page.Locator("h1").Filter(new() { HasText = "All Bookings" })
            );

            // Or use ILocator directly
            Heading = Page.GetByRole(AriaRole.Heading, new() { Name = "All Bookings" });
            NewBookingButton = Page.GetByRole(AriaRole.Button, new() { Name = "New Booking" });
            BookingsGrid = Page.Locator("[data-testid='bookings-grid']");
        }

        public override async Task GetAsync()
        {
            await Title.GetAsync();
            await HeadingAllBookings.GetAsync();
        }

        public override async Task VerifyAsync()
        {
            // Using TextElement verify methods
            var titleResult = await Title.VerifyTextAsync("Page Title", "All Bookings");
            Test.results.Add(titleResult);

            var headingResult = await HeadingAllBookings.VerifyVisibleAsync("Main Heading");
            Test.results.Add(headingResult);

            // Or use Playwright assertions directly
            await Assertions.Expect(Heading).ToBeVisibleAsync();
            await Assertions.Expect(Heading).ToHaveTextAsync("All Bookings");
        }
    }
}