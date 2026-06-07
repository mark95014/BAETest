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
        // Static lock dictionary: one lock per file path to prevent concurrent access to the same file
        private static readonly Dictionary<string, object> _fileLocks = new Dictionary<string, object>();
        private static readonly object _dictionaryLock = new object(); // Lock for accessing the dictionary itself
        
        private readonly List<string> labels = [];
        private readonly object _fileLock; // Instance-specific lock reference
        private bool first = true;
        public string FileName { get; }
        public string TestName;
        public readonly bool GenerateExpectedResults;

        public ExpectedResults(string testName, string expectedResultsFolder, bool generateExpectedResults)
        {
            TestName = testName;
            GenerateExpectedResults = generateExpectedResults;
            FileName = Path.Combine(expectedResultsFolder, $"{testName}.json");
            
            // Get or create a lock for this specific file
            lock (_dictionaryLock)
            {
                if (!_fileLocks.ContainsKey(FileName))
                {
                    _fileLocks[FileName] = new object();
                }
                _fileLock = _fileLocks[FileName];
            }
        }

        public void Init()
        {
            if (GenerateExpectedResults)
            {
                lock (_fileLock)
                {
                    var directory = Path.GetDirectoryName(FileName);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    File.WriteAllText(FileName, "{\n");
                    //first = true; // Reset for this file
                }
            }
        }

        public int Occurrences(string searchfor)
        {
            lock (_fileLock)
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
            lock (_fileLock)
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
            lock (_fileLock)
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
                lock (_fileLock)
                {
                    File.AppendAllText(FileName, "\n}");
                }
            }
        }
    }
}