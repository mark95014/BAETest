using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Threading.Tasks;
using BAETest.src.utils;

namespace BAETest.src.utils.PageData
{
    public abstract class BasePageData
    {
        protected IPage Page { get; private set; }

        public void Initialize(IPage page)
        {
            Page = page ?? throw new ArgumentNullException(nameof(page));
            InitializeElements();
        }

        protected abstract void InitializeElements();

        public abstract Task GetAsync();
        public abstract Task VerifyAsync();

        public async Task Get()
        {
            FieldInfo[] fields = GetType().GetFields();  // Simple - gets public fields

            foreach (FieldInfo field in fields)
            {
                if (field != null)
                {
                    MethodInfo method = field.FieldType.GetMethod("GetAsync");
                    if (method != null)
                    {
                        var task = (Task)method.Invoke(field.GetValue(this), new object[] { });
                        await task;
                    }
                }
            }
        }

        public void Verify(JObject expectedResult, string dataLabel)
        {
            FieldInfo[] fields = GetType().GetFields();  // Simple - gets public fields

            foreach (FieldInfo field in fields)
            {
                if (field != null && expectedResult[field.Name] != null)
                {
                    JObject expectedObject = (JObject)expectedResult[field.Name];
                    Object expected = expectedObject["data"];
                    MethodInfo VerifyMethod = field.FieldType.GetMethod("VerifyAsync");
                    if (VerifyMethod != null)
                    {
                        string dataName = dataLabel + "." + field.Name;
                        Result result = (Result)VerifyMethod.Invoke(field.GetValue(this), new object[] { dataName, expected });
                        BaseTest.results.Add(result);
                    }
                }
            }
        }
    }
}