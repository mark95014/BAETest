using Newtonsoft.Json;

namespace LDSTest.Shared
{
    /// <summary>
    /// This class looks up expected results per test case and can generate expected results from actual results.
    /// The expected results are stored as JSON files.
    /// Thread-safe for parallel test execution.
    /// </summary>
    public class ExpectedResults
    {
        private readonly List<string> labels = [];
        private readonly object _lock = new object(); // Single lock for both file and list operations
        private bool first = true;
        public string FileName { get; }
        public string TestName;
        public readonly bool GenerateExpectedResults;

        public ExpectedResults(string testName, string expectedResultsFolder, bool generateExpectedResults)
        {
            TestName = testName;
            GenerateExpectedResults = generateExpectedResults;
            FileName = Path.Combine(expectedResultsFolder, $"{testName}.json");
        }

        public void Init()
        {
            if (GenerateExpectedResults)
            {
                lock (_lock)
                {
                    var directory = Path.GetDirectoryName(FileName);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    File.WriteAllText(FileName, "{\n");
                }
            }
        }

        public int Occurrences(string searchfor)
        {
            lock (_lock)
            {
                return labels.Count(label => label.StartsWith(searchfor));
            }
        }

        public string MakeDataLabel(object data)
        {
            return MakeDataLabel(data.GetType().Name);
        }

        public string MakeDataLabel(string name)
        {
            lock (_lock)
            {
                string prefix = name + "." + TestCaseIdProvider.GetTestCaseId();
                string label;

                int count = labels.Count(l => l.StartsWith(prefix));

                if (count > 0) label = prefix + "." + count.ToString();
                else label = prefix;

                labels.Add(label);
                return label;
            }
        }

        public void Append(object data, string dataLabel)
        {
            lock (_lock)
            {
                var json = "";

                if (!first) json = ",\n";

                json += JsonConvert.SerializeObject(dataLabel) + ": ";
                json += JsonConvert.SerializeObject(data, Formatting.Indented);

                File.AppendAllText(FileName, json);
                first = false;
            }
        }

        public void Close()
        {
            if (GenerateExpectedResults)
            {
                lock (_lock)
                {
                    File.AppendAllText(FileName, "\n}");
                }
            }
        }
    }
}