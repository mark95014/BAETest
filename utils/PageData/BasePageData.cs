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

        public void Get()
        {
            foreach (FieldInfo field in GetType().GetFields())
            {
                //if (field != null && field.FieldType == typeof(Element))
                if (field != null)
                    {
                    MethodInfo method = field.FieldType.GetMethod("Get");
                    method.Invoke(field.GetValue(this), new object[] { });
                }
            }
        }

        public void Verify(JObject expectedResult, string dataLabel)
        {
            foreach (FieldInfo field in GetType().GetFields())
            {
                if (field != null)
                {
                    JObject expectedObject = (JObject)expectedResult[field.Name];
                    Object expected = expectedObject["data"];
                    MethodInfo VerifyMethod = field.FieldType.GetMethod("Verify");
                    string dataName = dataLabel + "." + field.Name;
                    Result result = (Result)VerifyMethod.Invoke(field.GetValue(this), new object[] { dataName, expected });
                    BaseTest.results.Add(result);
                }
            }
        }
    }
}