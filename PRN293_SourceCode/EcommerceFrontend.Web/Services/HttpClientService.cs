using System.Text;
using System.Text.Json;

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

    public HttpClientService(HttpClient httpClient, IConfiguration configuration, ILogger<HttpClientService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        var baseUrl = _configuration["ApiSettings:BaseUrl"];
        _logger.LogInformation("Initializing HttpClientService with base URL: {BaseUrl}", baseUrl);
        
        if (string.IsNullOrEmpty(baseUrl))
        {
            _logger.LogError("API Base URL is not configured!");
            throw new InvalidOperationException("API Base URL is not configured in appsettings.json");
        }

        _httpClient.BaseAddress = new Uri(baseUrl);
        _logger.LogInformation("HttpClient BaseAddress set to: {BaseAddress}", _httpClient.BaseAddress);

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
            _logger.LogInformation("Making GET request to {BaseUrl}{Endpoint}", _httpClient.BaseAddress, endpoint);
            
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Received response from {Endpoint}. Status: {StatusCode}, Content Length: {ContentLength}", 
                endpoint, response.StatusCode, content?.Length ?? 0);
            _logger.LogDebug("Response Content: {Content}", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("API request failed. Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, content);
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}. Content: {content}");
            }
            
            if (string.IsNullOrEmpty(content))
            {
                _logger.LogWarning("Empty response content from {Endpoint}", endpoint);
                return default;
            }

            var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for {BaseUrl}{Endpoint}. Status code: {StatusCode}", 
                _httpClient.BaseAddress, endpoint, ex.StatusCode);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response from {Endpoint}. Content: {Content}", 
                endpoint, await _httpClient.GetAsync(endpoint).Result.Content.ReadAsStringAsync());
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

            response.EnsureSuccessStatusCode();
            
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

            response.EnsureSuccessStatusCode();
            
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
            
            _logger.LogInformation("Received response from DELETE {Endpoint}. Status: {StatusCode}", 
                endpoint, response.StatusCode);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DELETE request to {Endpoint}", endpoint);
            throw;
        }
    }
} 