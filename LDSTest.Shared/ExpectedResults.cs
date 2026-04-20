using Newtonsoft.Json;

namespace LDSTest.Shared
{
    /// <summary>
    /// This class looks up expected results per test case and can generate expected results from actual results.
    /// The expected results are stored as JSON files.
    /// </summary>
    public class ExpectedResults
    {
        private readonly List<string> labels = [];
        private bool first = true;
        public string FileName { get; }
        public string TestName;
        public readonly bool GenerateExpectedResults;
        private readonly int testCaseId;

        public ExpectedResults(string testName, string expectedResultsFolder, bool generateExpectedResults)
        {
            TestName = testName;
            GenerateExpectedResults = generateExpectedResults;
            FileName = Path.Combine(expectedResultsFolder, $"{testName}.json");
            testCaseId = TestCaseIdProvider.GetTestCaseId();
        }

        public void Init()
        {
            if (GenerateExpectedResults) File.WriteAllText(FileName, "{\n"); // Start a new JSON object
        }

        public int Occurrences(string searchfor)
        {
            var count = 0;

            foreach (string label in labels)
            {
                if (label.StartsWith(searchfor)) count++;
            }

            return count;
        }

        public string MakeDataLabel(object data)
        {
            return MakeDataLabel(data.GetType().Name);
        }

        public string MakeDataLabel(string name)
        {
            string prefix = name + "." + testCaseId;
            string label;

            int count = Occurrences(prefix);

            if (count > 0) label = prefix + "." + count.ToString();
            else label = prefix;

            labels.Add(label);
            return label;
        }

        public void Append(object data, string dataLabel)
        {
            var json = "";

            if (!first) json = ",\n";

            json += JsonConvert.SerializeObject(dataLabel) + ": ";

            json += JsonConvert.SerializeObject(data, Formatting.Indented);

            File.AppendAllText(FileName, json);
            first = false;
        }

        public void Close()
        {
            if (GenerateExpectedResults)
            {
                File.AppendAllText(FileName, "\n}");
                first = true;
            }
        }
    }
}