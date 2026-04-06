using Microsoft.Playwright;


namespace LDSUITest.src.utils.PageData.Elements
{
    public class CsvElement : Element
    {
        private readonly string _downloadPath;
        private readonly string[] _columnsToIgnore;

        public CsvElement(ILocator downloadButtonLocator, string expectedFileName, string[] columnsToIgnore = null, string downloadPath = null)
            : base(downloadButtonLocator)
        {
            _downloadPath = downloadPath ?? Path.Combine(Path.GetTempPath(), "playwright-downloads");
            _columnsToIgnore = columnsToIgnore ?? Array.Empty<string>();
            Data = expectedFileName;
        }

        public override async Task GetAsync()
        {
            // Click the download button and wait for download
            var download = await Locator.Page.RunAndWaitForDownloadAsync(async () =>
            {
                await Locator.ClickAsync();
            });

            // Save the download
            var filePath = Path.Combine(_downloadPath, download.SuggestedFilename);
            await download.SaveAsAsync(filePath);

            // Read CSV content
            var csvData = await ReadCsvAsync(filePath);
            Data = csvData;
        }

        private async Task<List<List<string>>> ReadCsvAsync(string filePath)
        {
            var csvData = new List<List<string>>();

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split(',').Select(v => v.Trim('"', ' ')).ToList();

                    // Filter out ignored columns
                    var filteredValues = new List<string>();
                    for (int i = 0; i < values.Count; i++)
                    {
                        if (!_columnsToIgnore.Contains($"* {i}"))
                        {
                            filteredValues.Add(values[i]);
                        }
                    }

                    csvData.Add(filteredValues);
                }
            }

            return csvData;
        }

        public override async Task<Result> VerifyAsync(string name, object expected)
        {
            await GetAsync();

            if (expected is List<List<string>> expectedCsv)
            {
                var actualCsv = Data as List<List<string>>;

                if (actualCsv.Count != expectedCsv.Count)
                {
                    return new Result(false, $"{name}: CSV row count mismatch. Expected {expectedCsv.Count}, actual {actualCsv.Count}");
                }

                for (int i = 0; i < expectedCsv.Count; i++)
                {
                    for (int j = 0; j < expectedCsv[i].Count; j++)
                    {
                        if (actualCsv[i][j] != expectedCsv[i][j])
                        {
                            return new Result(false, $"{name}: CSV cell mismatch at [{i},{j}]. Expected '{expectedCsv[i][j]}', actual '{actualCsv[i][j]}'");
                        }
                    }
                }

                return new Result(true, $"{name}: CSV data matches");
            }

            return new Result(false, $"{name}: expected data is not in the correct format");
        }
    }
}
