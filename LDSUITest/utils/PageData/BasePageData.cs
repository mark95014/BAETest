using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace LDSUITest.utils.PageData
{
    public abstract class BasePageData
    {
        public IPage Page { get; set; } = null!;

        public void Initialize(IPage page)
        {
            Page = page ?? throw new ArgumentNullException(nameof(page));
            InitializeElements();
        }

        protected abstract void InitializeElements();

        public async Task Get()
        {
            FieldInfo[] fields = GetType().GetFields();

            foreach (FieldInfo field in fields)
            {
                if (field != null)
                {
                    MethodInfo method = field.FieldType.GetMethod("GetAsync")!;
                    if (method != null)
                    {
                        var task = (Task)method.Invoke(field.GetValue(this), [])!;
                        await task;
                    }
                }
            }
        }

        public async Task Verify(JObject expectedResult, string dataLabel)
        {
            FieldInfo[] fields = GetType().GetFields();  // gets public fields

            foreach (FieldInfo field in fields)
            {
                if (field != null && expectedResult[field.Name] != null)
                {
                    JObject expectedObject = (JObject)expectedResult[field.Name]!;
                    Object expected = expectedObject["Data"]!;
                    MethodInfo VerifyMethod = field.FieldType.GetMethod("VerifyAsync")!;
                    if (VerifyMethod != null)
                    {
                        string dataName = dataLabel + "." + field.Name;
                        var task = (Task<Result>)VerifyMethod.Invoke(field.GetValue(this), [dataName, expected])!;
                        Result result = await task;
                        BaseTest.Results.Add(result);
                    }
                }
            }
        }
    }
}