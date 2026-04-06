using Newtonsoft.Json;

public abstract class ExpectedResults
{
    static List<string> labels = new List<string>();
    static Boolean first = true;
    public static string fileName;
    //public static string ExpectedResultsFolder = Environment.GetEnvironmentVariable("ExpectedResultsFolder");
    public static string expectedResultsFolder = "../../../data/expectedResults";

    public static void Init(string testName, Boolean generateExpectedResults, string folder)
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
        string prefix = data.GetType().Name + "." + testCaseId;
        string label;

        int count = Occurrences(prefix);

        if (count > 0) label = prefix + "." + count.ToString();
        else label = prefix;

        labels.Add(label);
        return label;
    }

    public static void Append(Object data, string dataLabel)
    {
        var json = "";

        if (!first) json = ",\n";

        json += JsonConvert.SerializeObject(dataLabel) + ": ";

        json += JsonConvert.SerializeObject(data, Formatting.Indented);

        //    replacer: (k: string, v: any) => {
        //    if (k == "selector") return undefined
        //    if (k == "columnTypes") return undefined
        //    return v
        //}, maxLength: 200, indent: 4})

        File.AppendAllText(fileName, json);
        first = false;
    }

    public static void Close(Boolean generateExpectedResults)
    {
        if (generateExpectedResults)
        {
            File.AppendAllText(fileName, "\n}");
            first = true;
        }
    }
}