using Newtonsoft.Json;

namespace LDSTest.Shared
{
    public class ExpectedResults
    {
        private readonly List<string> labels = [];
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

        public string MakeDataLabel(object data, int testCaseId)
        {
            return MakeDataLabel(data.GetType().Name, testCaseId);
        }

        public string MakeDataLabel(string name, int testCaseId)
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