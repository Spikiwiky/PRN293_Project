using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace EcommerceFrontend.Web.Services;

public interface IHttpClientService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task DeleteAsync(string endpoint);
}

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HttpClientService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpClientService(HttpClient httpClient, IConfiguration configuration, ILogger<HttpClientService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;

        var baseUrl = _configuration["ApiSettings:BaseUrl"];
        _logger.LogInformation("Initializing HttpClientService with base URL: {BaseUrl}", baseUrl);
        
        if (string.IsNullOrEmpty(baseUrl))
        {
            _logger.LogError("API Base URL is not configured!");
            throw new InvalidOperationException("API Base URL is not configured in appsettings.json");
        }

        _httpClient.BaseAddress = new Uri(baseUrl);
        _logger.LogInformation("HttpClient BaseAddress set to: {BaseAddress}", _httpClient.BaseAddress);

        // Add default headers for CORS
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "EcommerceFrontend/1.0");

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            // Ensure endpoint starts with / but doesn't have double slashes
            var cleanEndpoint = endpoint.TrimStart('/');
            cleanEndpoint = "/" + cleanEndpoint;
            
            // Remove any double slashes that might occur
            cleanEndpoint = cleanEndpoint.Replace("//", "/");
            
            _logger.LogInformation("Making GET request to {Endpoint}", cleanEndpoint);
            
            // Create request with cookies
            var request = new HttpRequestMessage(HttpMethod.Get, cleanEndpoint);
            
            // Add cookies from current context
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var cookies = httpContext.Request.Cookies;
                var cookieHeader = string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}"));
                if (!string.IsNullOrEmpty(cookieHeader))
                {
                    request.Headers.Add("Cookie", cookieHeader);
                    _logger.LogInformation("Added cookies to request: {Cookies}", cookieHeader);
                }
            }
            
            using var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Received response from {Endpoint}. Status: {StatusCode}, Content Length: {ContentLength}", 
                cleanEndpoint, response.StatusCode, content?.Length ?? 0);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"API request failed with status code {response.StatusCode}.";
                if (!string.IsNullOrEmpty(content))
                {
                    errorMessage += $" Content: {content}";
                }

                _logger.LogError("API Error: {ErrorMessage}", errorMessage);
                _logger.LogError("Request URL: {BaseUrl}{Endpoint}", _httpClient.BaseAddress, cleanEndpoint);
                _logger.LogError("Response Headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}")));

                throw new HttpRequestException(errorMessage, null, response.StatusCode);
            }
            
            if (string.IsNullOrEmpty(content))
            {
                _logger.LogWarning("Empty response content from {Endpoint}", cleanEndpoint);
                return default;
            }

            try
            {
                var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                _logger.LogDebug("Successfully deserialized response to type {Type}", typeof(T).Name);
                return result;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response. Content: {Content}", content);
                _logger.LogError("Expected Type: {Type}, Content Type: {ContentType}", 
                    typeof(T).Name, response.Content.Headers.ContentType?.MediaType);
                throw;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for {BaseUrl}{Endpoint}. Status code: {StatusCode}", 
                _httpClient.BaseAddress, endpoint, ex.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling {BaseUrl}{Endpoint}. Error: {Error}", 
                _httpClient.BaseAddress, endpoint, ex.Message);
            throw;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogInformation("Making POST request to {Endpoint} with data: {Data}", endpoint, json);
            
            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Received response from POST {Endpoint}. Status: {StatusCode}, Content: {Content}", 
                endpoint, response.StatusCode, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"API request failed with status code {response.StatusCode}.";
                if (!string.IsNullOrEmpty(responseContent))
                {
                    errorMessage += $" Content: {responseContent}";
                }
                throw new HttpRequestException(errorMessage, null, response.StatusCode);
            }
            
            if (string.IsNullOrEmpty(responseContent))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in POST request to {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogInformation("Making PUT request to {Endpoint} with data: {Data}", endpoint, json);
            
            var response = await _httpClient.PutAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Received response from PUT {Endpoint}. Status: {StatusCode}, Content: {Content}", 
                endpoint, response.StatusCode, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"API request failed with status code {response.StatusCode}.";
                if (!string.IsNullOrEmpty(responseContent))
                {
                    errorMessage += $" Content: {responseContent}";
                }
                throw new HttpRequestException(errorMessage, null, response.StatusCode);
            }
            
            if (string.IsNullOrEmpty(responseContent))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PUT request to {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task DeleteAsync(string endpoint)
    {
        try
        {
            _logger.LogInformation("Making DELETE request to {Endpoint}", endpoint);
            
            var response = await _httpClient.DeleteAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Received response from DELETE {Endpoint}. Status: {StatusCode}", 
                endpoint, response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"API request failed with status code {response.StatusCode}.";
                if (!string.IsNullOrEmpty(responseContent))
                {
                    errorMessage += $" Content: {responseContent}";
                }
                throw new HttpRequestException(errorMessage, null, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DELETE request to {Endpoint}", endpoint);
            throw;
        }
    }
} 