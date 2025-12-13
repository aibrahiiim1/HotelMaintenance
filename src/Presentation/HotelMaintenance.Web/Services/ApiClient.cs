using HotelMaintenance.Web.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HotelMaintenance.Web.Services;

/// <summary>
/// Service for calling the Hotel Maintenance REST API
/// </summary>
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Set JWT token for authenticated requests
    /// </summary>
    private void SetAuthToken()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    /// <summary>
    /// GET request
    /// </summary>
    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            SetAuthToken();
            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }

            _logger.LogWarning("API GET request failed: {StatusCode} - {Endpoint}",
                response.StatusCode, endpoint);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API GET: {Endpoint}", endpoint);
            return default;
        }
    }

    /// <summary>
    /// POST request
    /// </summary>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            SetAuthToken();
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(responseContent);
            }

            _logger.LogWarning("API POST request failed: {StatusCode} - {Endpoint}",
                response.StatusCode, endpoint);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API POST: {Endpoint}", endpoint);
            return default;
        }
    }

    /// <summary>
    /// PUT request
    /// </summary>
    public async Task<bool> PutAsync<TRequest>(string endpoint, TRequest data)
    {
        try
        {
            SetAuthToken();
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API PUT: {Endpoint}", endpoint);
            return false;
        }
    }

    /// <summary>
    /// DELETE request
    /// </summary>
    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            SetAuthToken();
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API DELETE: {Endpoint}", endpoint);
            return false;
        }
    }

    /// <summary>
    /// Login to the API
    /// </summary>
    public async Task<(bool success, string? token, UserInfoViewModel? user, string? error)> LoginAsync(string email, string password)
    {
        try
        {
            var loginData = new { email, password };
            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/Authentication/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                var token = loginResponse?.token?.ToString();
                var user = JsonConvert.DeserializeObject<UserInfoViewModel>(loginResponse?.user?.ToString() ?? "{}");

                return (true, token, user, null);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return (false, null, null, "Invalid email or password");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return (false, null, null, "An error occurred during login");
        }
    }
}

/// <summary>
/// Extension methods for API Client
/// </summary>
public static class ApiClientExtensions
{
    public static IServiceCollection AddApiClient(this IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient<ApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}