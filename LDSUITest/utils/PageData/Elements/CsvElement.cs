using LDSTest.Shared;
using OpenQA.Selenium;
using System.IO;


namespace LDSUITest.utils.PageData.Elements
{
    public class CsvElement : Element
    {
        private readonly string _downloadPath;
        private readonly string[] _columnsToIgnore;

        public CsvElement(
            IWebDriver driver,
            By locator,
            string downloadPath,
            string defaultValue = "", 
            string? excludeColumns = null)
            : base(driver, locator)
        {
            _downloadPath = downloadPath ?? Path.Combine(Path.GetTempPath(), "selenium-downloads");
            _columnsToIgnore = excludeColumns?.Split(',')?.Select(c => c.Trim()).ToArray() ?? Array.Empty<string>();
            Data = defaultValue;

            // Ensure download directory exists
            if (!Directory.Exists(_downloadPath))
            {
                Directory.CreateDirectory(_downloadPath);
            }
        }

        public override void Get()
        {
            // Clear old files before download
            ClearOldDownloads();

            // Get file count before clicking
            var filesBefore = Directory.GetFiles(_downloadPath, "*.csv").Length;

            // Click the download button
            Driver.FindElement(Locator).Click();
            
            // Wait for file to download
            var downloadedFile = WaitForDownload(filesBefore);
            
            if (downloadedFile != null)
            {
                var csvData = ReadCsv(downloadedFile);
                Data = csvData;
            }
            else
            {
                Data = new List<List<string>>();
            }
        }

        private void ClearOldDownloads()
        {
            try
            {
                var oldFiles = Directory.GetFiles(_downloadPath, "*.csv")
                    .Where(f => (DateTime.Now - File.GetCreationTime(f)).TotalMinutes > 5);
                
                foreach (var file in oldFiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // Ignore errors deleting old files
                    }
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        private string? WaitForDownload(int previousFileCount, int timeoutSeconds = 30)
        {
            var startTime = DateTime.Now;
            string? latestFile = null;

            while ((DateTime.Now - startTime).TotalSeconds < timeoutSeconds)
            {
                try
                {
                    var currentFiles = Directory.GetFiles(_downloadPath, "*.csv");

                    // Check if a new file appeared
                    if (currentFiles.Length > previousFileCount)
                    {
                        // Get the most recently created CSV file
                        latestFile = currentFiles
                            .OrderByDescending(f => File.GetCreationTime(f))
                            .FirstOrDefault();

                        if (latestFile != null)
                        {
                            // Wait for file to finish downloading
                            // Check if file is not being written to anymore
                            if (IsFileReady(latestFile))
                            {
                                return latestFile;
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }

            return latestFile;
        }

        private bool IsFileReady(string filePath, int maxAttempts = 10)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        return true;
                    }
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                }
            }
            return true; // Assume ready after max attempts
        }

        private List<List<string>> ReadCsv(string filePath)
        {
            var csvData = new List<List<string>>();

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line)) continue;

                        var values = ParseCsvLine(line);

                        // Filter out ignored columns
                        var filteredValues = new List<string>();
                        for (int i = 0; i < values.Count; i++)
                        {
                            if (!_columnsToIgnore.Contains($"* {i}"))
                            {
                                filteredValues.Add(values[i] ?? string.Empty);
                            }
                        }

                        csvData.Add(filteredValues);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading CSV file: {ex.Message}", ex);
            }

            return csvData;
        }

        private List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            var currentValue = new System.Text.StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // Escaped quote
                        currentValue.Append('"');
                        i++; // Skip next quote
                    }
                    else
                    {
                        // Toggle quote state
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    // End of field
                    values.Add(currentValue.ToString().Trim());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            // Add the last value
            values.Add(currentValue.ToString().Trim());

            return values;
        }

        public override Result Verify(string name, object expected)
        {
            Get();

            if (expected is List<List<string>> expectedCsv)
            {
                var actualCsv = Data as List<List<string>>;

                if (actualCsv == null)
                {
                    return new Result(false, $"{name}: CSV data is null");
                }

                if (actualCsv.Count != expectedCsv.Count)
                {
                    return new Result(false, $"{name}: CSV row count mismatch. Expected {expectedCsv.Count}, actual {actualCsv.Count}");
                }

                for (int i = 0; i < expectedCsv.Count; i++)
                {
                    if (i >= actualCsv.Count)
                    {
                        return new Result(false, $"{name}: Missing row {i} in actual CSV");
                    }

                    if (actualCsv[i].Count != expectedCsv[i].Count)
                    {
                        return new Result(false, $"{name}: Row {i} column count mismatch. Expected {expectedCsv[i].Count}, actual {actualCsv[i].Count}");
                    }

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

            return new Result(false, $"{name}: expected data is not in the correct format (List<List<string>>)");
        }
    }
}
