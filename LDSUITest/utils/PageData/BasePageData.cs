using LDSTest.Shared;
using Microsoft.Playwright;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace LDSUITest.utils.PageData
{
    public abstract class BasePageData
    {
        [JsonIgnore]
        public IPage Page { get; set; } = null!;

        public void Initialize(IPage page)
        {
            Page = page ?? throw new ArgumentNullException(nameof(page));
            InitializeElements();
        }

        protected abstract void InitializeElements();

        public async Task Get()
        {
            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property != null && property.Name != nameof(Page))
                {
                    MethodInfo? method = property.PropertyType.GetMethod("GetAsync");
                    if (method != null)
                    {
                        var task = (Task)method.Invoke(property.GetValue(this), [])!;
                        await task;
                    }
                }
            }
        }

        public async Task Verify(JObject expectedResult, string dataLabel, Results results)
        {
            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property != null && property.Name != nameof(Page) && expectedResult[property.Name] != null)
                {
                    JObject expectedObject = (JObject)expectedResult[property.Name]!;
                    Object expected = expectedObject["Data"]!;
                    MethodInfo? verifyMethod = property.PropertyType.GetMethod("VerifyAsync");
                    if (verifyMethod != null)
                    {
                        string dataName = dataLabel + "." + property.Name;
                        var task = (Task<Result>)verifyMethod.Invoke(property.GetValue(this), [dataName, expected])!;
                        Result result = await task;
                        results.Add(result);
                    }
                }
            }
        }
    }
}