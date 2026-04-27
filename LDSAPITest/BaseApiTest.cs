using LDSTest.Shared;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace LDSAPITest
{
    /// <summary>
    /// Base class for all API tests. Provides HttpClient configuration, authentication,
    /// and common helper methods.
    /// </summary>
    public abstract class BaseApiTest
    {
        public required ExpectedResults ExpectedResults;
        public required Results Results;
        public required string TestName;

        // Implement in the future. public Results Results { get; } = new Results();

        protected HttpClient HttpClient { get; private set; } = null!;
        protected string ApiBaseUrl { get; private set; } = null!;
        protected string Environment { get; private set; } = null!;
        protected int ApiTimeout { get; private set; }
        protected bool Verbose { get; private set; }
        public bool GenerateExpectedResults { get; private set; }

        [OneTimeSetUp]
        public virtual void BaseOneTimeSetUp()
        {
            // Read test parameters from .runsettings
            Environment = TestContext.Parameters["environment"]!;
            Verbose = bool.Parse(TestContext.Parameters["verbose"]!);
            GenerateExpectedResults = bool.Parse(TestContext.Parameters["generateExpectedResults"]!);
            ApiTimeout = int.Parse(TestContext.Parameters["apiTimeout"]!);
            ApiBaseUrl = TestContext.Parameters[$"{Environment}.apiBaseURL"]!;

            LogInfo("=== API Test Setup ===");
            LogInfo($"Environment: {Environment}");
            LogInfo($"API Base URL: {ApiBaseUrl}");
            LogInfo($"Timeout: {ApiTimeout}s");

            // Initialize HttpClient
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseUrl),
                Timeout = TimeSpan.FromSeconds(ApiTimeout)
            };

            // Set default headers
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "LDSAPITest/1.0");

            ConfigureAuthentication();

            TestName = TestContext.CurrentContext.Test.Name;

            var expectedResultsFolder = TestContext.Parameters["expectedResultsFolder"] ?? "../../../data/expectedResults";
            ExpectedResults = new ExpectedResults(TestName, expectedResultsFolder, GenerateExpectedResults);
            ExpectedResults.Init();

            //new Database().ResetDatabase().GetAwaiter().GetResult();  manually reset the database
        }

        [SetUp]
        public virtual void TestCaseSetUp()
        {
            Results = new Results();
        }

        [OneTimeTearDown]
        public virtual void BaseOneTimeTearDown()
        {
            //new Database().ResetDatabase().GetAwaiter().GetResult();
            ExpectedResults.Close();
            HttpClient?.Dispose();
            LogInfo("=== API Test Teardown Complete ===");
        }

        /// <summary>
        /// Configure authentication based on .runsettings parameters.
        /// Override this method in derived classes for custom authentication.
        /// </summary>
        protected virtual void ConfigureAuthentication()
        {
            var authType = TestContext.Parameters["apiAuthType"]!;

            LogInfo($"Authentication Type: {authType}");

            switch (authType.ToLowerInvariant())
            {
                case "basic":
                    ConfigureBasicAuth();
                    break;
                case "bearer":
                    ConfigureBearerAuth();
                    break;
                case "apikey":
                    ConfigureApiKeyAuth();
                    break;
                case "none":
                    LogInfo("No authentication configured");
                    break;
                default:
                    LogInfo($"Unknown auth type: {authType}, skipping authentication");
                    break;
            }
        }

        private void ConfigureBasicAuth()
        {
            var username = TestContext.Parameters["apiUsername"];
            var password = TestContext.Parameters["apiPassword"];

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                LogInfo($"Basic authentication configured for user: {username}");
            }
            else
            {
                LogInfo("Warning: Basic auth selected but username/password not provided");
            }
        }

        private void ConfigureBearerAuth()
        {
            var token = TestContext.Parameters["apiBearerToken"];

            if (!string.IsNullOrEmpty(token))
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                LogInfo("Bearer token authentication configured");
            }
            else
            {
                LogInfo("Warning: Bearer auth selected but token not provided");
            }
        }

        private void ConfigureApiKeyAuth()
        {
            var headerName = TestContext.Parameters["apiKeyHeaderName"]!;
            var apiKey = TestContext.Parameters["apiKeyValue"]!;

            if (!string.IsNullOrEmpty(apiKey))
            {
                HttpClient.DefaultRequestHeaders.Add(headerName, apiKey);
                LogInfo($"API Key authentication configured with header: {headerName}");
            }
            else
            {
                LogInfo("Warning: API Key auth selected but key not provided");
            }
        }

        #region HTTP Helper Methods

        /// <summary>
        /// Sends a GET request.
        /// </summary>
        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            LogInfo($"GET {endpoint}");
            var response = await HttpClient.GetAsync(endpoint);
            LogResponse(response);
            return response;
        }

        /// <summary>
        /// Sends a POST request.
        /// </summary>
        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T content)
        {
            LogInfo($"POST {endpoint}");
            var json = JsonConvert.SerializeObject(content, Formatting.Indented);
            LogInfo($"Request Body:\n{json}");

            using var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync(endpoint, httpContent);
            LogResponse(response);
            return response;
        }

        /// <summary>
        /// Sends a PUT request.
        /// </summary>
        public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T content)
        {
            LogInfo($"PUT {endpoint}");
            var json = JsonConvert.SerializeObject(content, Formatting.Indented);
            LogInfo($"Request Body:\n{json}");

            using var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await HttpClient.PutAsync(endpoint, httpContent);
            LogResponse(response);
            return response;
        }

        /// <summary>
        /// Sends a PATCH request.
        /// </summary>
        public async Task<HttpResponseMessage> PatchAsync<T>(string endpoint, T content)
        {
            LogInfo($"PATCH {endpoint}");
            var json = JsonConvert.SerializeObject(content, Formatting.Indented);
            LogInfo($"Request Body:\n{json}");

            using var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await HttpClient.PatchAsync(endpoint, httpContent);
            LogResponse(response);
            return response;
        }

        /// <summary>
        /// Sends a DELETE request.
        /// </summary>
        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            LogInfo($"DELETE {endpoint}");
            var response = await HttpClient.DeleteAsync(endpoint);
            LogResponse(response);
            return response;
        }

        #endregion

        #region Response Helper Methods

        /// <summary>
        /// Deserializes the response content to the specified type.
        /// </summary>
        //public async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
        //{
        //    var content = await response.Content.ReadAsStringAsync();

        //    if (Verbose)
        //    {
        //        LogInfo($"Response Content:\n{content}");
        //    }

        //    if (string.IsNullOrWhiteSpace(content))
        //    {
        //        return default;
        //    }

        //    try
        //    {
        //        return JsonConvert.DeserializeObject<T>(content);
        //    }
        //    catch (JsonException ex)
        //    {
        //        LogInfo($"Failed to deserialize response: {ex.Message}");
        //        throw;
        //    }
        //}

        /// <summary>
        /// Gets the response content as a string.
        /// </summary>
        public async Task<string> GetResponseContentAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (Verbose)
            {
                LogInfo($"Response Content:\n{content}");
            }

            return content;
        }

        /// <summary>
        /// Asserts that the response has the expected status code.
        /// </summary>
        public static async Task AssertStatusCodeAsync(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
        {
            if (response.StatusCode != expectedStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Assert.Fail(
                    $"Expected status code {(int)expectedStatusCode} ({expectedStatusCode}), " +
                    $"but got {(int)response.StatusCode} ({response.StatusCode}).\n" +
                    $"Response content: {content}");
            }
        }

        /// <summary>
        /// Ensures the response was successful (2xx status code).
        /// </summary>
        public static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Request failed with status {(int)response.StatusCode} ({response.StatusCode}).\n" +
                    $"Response content: {content}");
            }
        }

        #endregion

        #region Logging Methods

        /// <summary>
        /// Logs an informational message if verbose mode is enabled.
        /// </summary>
        public void LogInfo(string message)
        {
            if (Verbose)
            {
                TestContext.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");
            }
        }

        /// <summary>
        /// Logs the HTTP response status and headers.
        /// </summary>
        public void LogResponse(HttpResponseMessage response)
        {
            LogInfo($"Response Status: {(int)response.StatusCode} {response.StatusCode}");

            if (Verbose && response.Headers.Any())
            {
                LogInfo("Response Headers:");
                foreach (var header in response.Headers)
                {
                    LogInfo($"  {header.Key}: {string.Join(", ", header.Value)}");
                }
            }
        }

        #endregion
    }
}