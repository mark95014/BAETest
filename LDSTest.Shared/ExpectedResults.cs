using Newtonsoft.Json;

namespace LDSTest.Shared
{
    public abstract class ExpectedResults
    {
        // Make static fields private and expose via properties if needed
        private static List<string> labels = new();
        private static bool first = true;
        private static string fileName = string.Empty;
        private static readonly string expectedResultsFolder = "../../../data/expectedResults";

        // Add public static properties if external access is required
        public static string FileName => fileName;
        public static string ExpectedResultsFolder => expectedResultsFolder;

        public static void Init(string testName, bool generateExpectedResults, string folder)
        {
            fileName = expectedResultsFolder + "/" + folder + "/" + testName + ".json";
            if (generateExpectedResults) File.WriteAllText(fileName, "{\n");
        }

        public static int Occurrences(string searchfor)
        {
            var count = 0;

            foreach (string label in labels)
            {
                if (label.StartsWith(searchfor)) count++;
            }

            return count;
        }

        public static string MakeDataLabel(object data, int testCaseId)
        {
            return MakeDataLabel(data.GetType().Name, testCaseId);
        }

        public static string MakeDataLabel(string name, int testCaseId)
        {
            string prefix = name + "." + testCaseId;
            string label;

            int count = Occurrences(prefix);

            if (count > 0) label = prefix + "." + count.ToString();
            else label = prefix;

            labels.Add(label);
            return label;
        }

        public static void Append(object data, string dataLabel)
        {
            if (fileName == null)
                throw new InvalidOperationException("ExpectedResults.Init must be called before Append");

            var json = "";

            if (!first) json = ",\n";

            json += JsonConvert.SerializeObject(dataLabel) + ": ";

            json += JsonConvert.SerializeObject(data, Formatting.Indented);

            File.AppendAllText(fileName, json);
            first = false;
        }

        public static void Close(bool generateExpectedResults)
        {
            if (generateExpectedResults && fileName != null)
            {
                File.AppendAllText(fileName, "\n}");
                first = true;
            }
        }
    }
}