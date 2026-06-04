using LDSTest.Shared;
using OpenQA.Selenium;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace LDSUITest.utils.PageData
{
    public abstract class BasePageData
    {
        [JsonIgnore]
        public IWebDriver Driver { get; set; } = null!;

        public void Initialize(IWebDriver driver)
        {
            Driver = driver ?? throw new ArgumentNullException(nameof(driver));
            InitializeElements();
        }

        protected abstract void InitializeElements();

        public void Get()
        {
            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property != null && property.Name != nameof(Driver))
                {
                    MethodInfo? method = property.PropertyType.GetMethod("Get");
                    if (method != null)
                    {
                        method.Invoke(property.GetValue(this), []);
                    }
                }
            }
        }

        public void Verify(JObject expectedResult, string dataLabel, Results results)
        {
            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property != null && property.Name != nameof(Driver) && expectedResult[property.Name] != null)
                {
                    JObject expectedObject = (JObject)expectedResult[property.Name]!;
                    Object expected = expectedObject["Data"]!;
                    MethodInfo? verifyMethod = property.PropertyType.GetMethod("Verify");
                    if (verifyMethod != null)
                    {
                        string dataName = dataLabel + "." + property.Name;
                        Result result = (Result)verifyMethod.Invoke(property.GetValue(this), [dataName, expected])!;
                        results.Add(result);
                    }
                }
            }
        }
    }
}