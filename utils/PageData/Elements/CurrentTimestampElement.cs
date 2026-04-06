using Microsoft.Playwright;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LDSTest.src.utils.PageData.Elements
{
    public class CurrentTimestampElement : TextElement
    {
        public CurrentTimestampElement(ILocator locator) : base(locator)
        {
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            await GetAsync();
            
            string actualTimestamp = Data?.ToString() ?? "";
            
            // Extract date components (assuming format like "12/25/2023 10:30 AM" or similar)
            var timestampRegex = new Regex(@"(\d{1,2})/(\d{1,2})/(\d{4})");
            var match = timestampRegex.Match(actualTimestamp);
            
            if (!match.Success)
            {
                return new Result(false, $"{name}: timestamp format invalid - '{actualTimestamp}'");
            }

            // Verify it's within a reasonable time range (e.g., within last 24 hours)
            var today = DateTime.Today;
            
            try
            {
                int month = int.Parse(match.Groups[1].Value);
                int day = int.Parse(match.Groups[2].Value);
                int year = int.Parse(match.Groups[3].Value);
                
                var timestampDate = new DateTime(year, month, day);
                var daysDifference = (today - timestampDate).TotalDays;
                
                if (daysDifference < 0 || daysDifference > 1)
                {
                    return new Result(false, $"{name}: timestamp date '{timestampDate:d}' is not recent (today is {today:d})");
                }
                
                return new Result(true, $"{name}: timestamp is current - '{actualTimestamp}'");
            }
            catch (Exception ex)
            {
                return new Result(false, $"{name}: error parsing timestamp '{actualTimestamp}': {ex.Message}");
            }
        }
    }
}