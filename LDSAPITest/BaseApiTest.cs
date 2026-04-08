using LDSTest.Shared;
using Newtonsoft.Json;
using NUnit.Framework;
using Polly;
using Polly.Retry;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace LDSAPITest;

/// <summary>
/// Base class for all API tests. Provides HttpClient configuration, authentication,
/// retry logic, and common helper methods.
/// </summary>
public abstract class BaseApiTest
{
    protected HttpClient HttpClient { get; private set; } = null!;
    protected string ApiBaseUrl { get; private set; } = null!;
    protected string Environment { get; private set; } = null!;
    protected int ApiTimeout { get; private set; }
    protected int ApiRetryCount { get; private set; }
    protected int ApiRetryDelayMs { get; private set; }
    protected bool Verbose { get; private set; }
    public static bool GenerateExpectedResults { get; private set; }

    protected AsyncRetryPolicy<HttpResponseMessage> RetryPolicy { get; private set; } = null!;

    [OneTimeSetUp]
    public virtual void BaseOneTimeSetUp()
    {
   
        // Read test parameters from .runsettings
        Environment = TestContext.Parameters["environment"] ?? "qa";
        Verbose = bool.Parse(TestContext.Parameters["verbose"] ?? "false");
        GenerateExpectedResults = bool.Parse(TestContext.Parameters["generateExpectedResults"] ?? "false");
        ApiTimeout = int.Parse(TestContext.Parameters["apiTimeout"] ?? "30");
        ApiRetryCount = int.Parse(TestContext.Parameters["apiRetryCount"] ?? "3");
        ApiRetryDelayMs = int.Parse(TestContext.Parameters["apiRetryDelayMs"] ?? "1000");

        // Get environment-specific API base URL
        ApiBaseUrl = TestContext.Parameters[$"{Environment}.apiBaseURL"] 
                     ?? TestContext.Parameters["apiBaseURL"] 
                     ?? throw new InvalidOperationException($"API Base URL not configured for environment: {Environment}");

        LogInfo($"=== API Test Setup ===");
        LogInfo($"Environment: {Environment}");
        LogInfo($"API Base URL: {ApiBaseUrl}");
        LogInfo($"Timeout: {ApiTimeout}s");
        LogInfo($"Retry Count: {ApiRetryCount}");

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

        // Configure authentication
        ConfigureAuthentication();

        // Configure retry policy using Polly
        RetryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => 
                r.StatusCode == HttpStatusCode.RequestTimeout ||
                r.StatusCode == HttpStatusCode.ServiceUnavailable ||
                r.StatusCode == HttpStatusCode.GatewayTimeout ||
                (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(
                ApiRetryCount,
                retryAttempt => TimeSpan.FromMilliseconds(ApiRetryDelayMs * retryAttempt),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    LogInfo($"Retry {retryCount} after {timespan.TotalSeconds}s due to: {outcome.Result.StatusCode}");
                });

        new Database().ResetDatabase().GetAwaiter().GetResult();
        ExpectedResults.Init(GetType().Name, GenerateExpectedResults, "regression");
    }

    [OneTimeTearDown]
    public virtual void BaseOneTimeTearDown()
    {
        ExpectedResults.Close(GenerateExpectedResults);
        HttpClient?.Dispose();
        LogInfo("=== API Test Teardown Complete ===");
        new Database().ResetDatabase().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Configure authentication based on .runsettings parameters.
    /// Override this method in derived classes for custom authentication.
    /// </summary>
    protected virtual void ConfigureAuthentication()
    {
        var authType = TestContext.Parameters["apiAuthType"] ?? "None";
        
        LogInfo($"Authentication Type: {authType}");

        switch (authType.ToLower())
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
        var headerName = TestContext.Parameters["apiKeyHeaderName"] ?? "X-API-Key";
        var apiKey = TestContext.Parameters["apiKeyValue"];
        
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
    /// Sends a GET request with automatic retry on transient failures.
    /// </summary>
    protected async Task<HttpResponseMessage> GetAsync(string endpoint, bool useRetry = true)
    {
        LogInfo($"GET {endpoint}");
        
        if (useRetry)
        {
            return await RetryPolicy.ExecuteAsync(async () => await HttpClient.GetAsync(endpoint));
        }
        
        var response = await HttpClient.GetAsync(endpoint);
        LogResponse(response);
        return response;
    }

    /// <summary>
    /// Sends a POST request with automatic retry on transient failures.
    /// </summary>
    protected async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T content, bool useRetry = false)
    {
        LogInfo($"POST {endpoint}");
        var json = JsonConvert.SerializeObject(content, Formatting.Indented);
        LogInfo($"Request Body:\n{json}");
        
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        
        if (useRetry)
        {
            return await RetryPolicy.ExecuteAsync(async () => await HttpClient.PostAsync(endpoint, httpContent));
        }
        
        var response = await HttpClient.PostAsync(endpoint, httpContent);
        LogResponse(response);
        return response;
    }

    /// <summary>
    /// Sends a PUT request with automatic retry on transient failures.
    /// </summary>
    protected async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T content, bool useRetry = false)
    {
        LogInfo($"PUT {endpoint}");
        var json = JsonConvert.SerializeObject(content, Formatting.Indented);
        LogInfo($"Request Body:\n{json}");
        
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        
        if (useRetry)
        {
            return await RetryPolicy.ExecuteAsync(async () => await HttpClient.PutAsync(endpoint, httpContent));
        }
        
        var response = await HttpClient.PutAsync(endpoint, httpContent);
        LogResponse(response);
        return response;
    }

    /// <summary>
    /// Sends a PATCH request with automatic retry on transient failures.
    /// </summary>
    protected async Task<HttpResponseMessage> PatchAsync<T>(string endpoint, T content, bool useRetry = false)
    {
        LogInfo($"PATCH {endpoint}");
        var json = JsonConvert.SerializeObject(content, Formatting.Indented);
        LogInfo($"Request Body:\n{json}");
        
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        
        if (useRetry)
        {
            return await RetryPolicy.ExecuteAsync(async () => await HttpClient.PatchAsync(endpoint, httpContent));
        }
        
        var response = await HttpClient.PatchAsync(endpoint, httpContent);
        LogResponse(response);
        return response;
    }

    /// <summary>
    /// Sends a DELETE request with automatic retry on transient failures.
    /// </summary>
    protected async Task<HttpResponseMessage> DeleteAsync(string endpoint, bool useRetry = false)
    {
        LogInfo($"DELETE {endpoint}");
        
        if (useRetry)
        {
            return await RetryPolicy.ExecuteAsync(async () => await HttpClient.DeleteAsync(endpoint));
        }
        
        var response = await HttpClient.DeleteAsync(endpoint);
        LogResponse(response);
        return response;
    }

    #endregion

    #region Response Helper Methods

    /// <summary>
    /// Deserializes the response content to the specified type.
    /// </summary>
    protected async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        
        if (Verbose)
        {
            LogInfo($"Response Content:\n{content}");
        }
        
        if (string.IsNullOrWhiteSpace(content))
        {
            return default;
        }
        
        try
        {
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch (JsonException ex)
        {
            LogInfo($"Failed to deserialize response: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Gets the response content as a string.
    /// </summary>
    protected async Task<string> GetResponseContentAsync(HttpResponseMessage response)
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
    protected void AssertStatusCode(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
    {
        if (response.StatusCode != expectedStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            Assert.Fail(
                $"Expected status code {(int)expectedStatusCode} ({expectedStatusCode}), " +
                $"but got {(int)response.StatusCode} ({response.StatusCode}).\n" +
                $"Response content: {content}");
        }
    }

    /// <summary>
    /// Ensures the response was successful (2xx status code).
    /// </summary>
    protected async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
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
    protected void LogInfo(string message)
    {
        if (Verbose)
        {
            TestContext.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");
        }
    }

    /// <summary>
    /// Logs the HTTP response status and headers.
    /// </summary>
    protected void LogResponse(HttpResponseMessage response)
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

    public static int GetTestCaseId()
    {
        var arg = TestContext.CurrentContext.Test.Arguments[0];
        string testCaseIdString = arg?.ToString() ?? throw new InvalidOperationException("Test case ID argument is null.");
        return int.Parse(testCaseIdString);
    }

    #endregion
}