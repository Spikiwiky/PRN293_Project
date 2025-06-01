using EcommerceFrontend.Web.Models.DTOs;
using System.Text.Json;

namespace EcommerceFrontend.Web.Services;

public interface IProductService
{
    Task<List<ProductDTO>> GetAllProductsAsync(int page = 1, int pageSize = 10);
    Task<List<ProductDTO>> SearchProductsAsync(string? name = null, string? category = null, 
        string? size = null, string? color = null, string? variantId = null, 
        decimal? price = null, int page = 1, int pageSize = 10);
}

public class ProductService : IProductService
{
    private readonly IHttpClientService _httpClient;
    private readonly ILogger<ProductService> _logger;
    private readonly IConfiguration _configuration;
    private const string BaseEndpoint = "/api/Product";

    public ProductService(IHttpClientService httpClient, ILogger<ProductService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<List<ProductDTO>> GetAllProductsAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var apiUrl = _configuration["ApiSettings:BaseUrl"];
            _logger.LogInformation("API Base URL configured as: {ApiUrl}", apiUrl);
            _logger.LogInformation("Calling API to get all products. Endpoint: {Endpoint}, Page: {Page}, PageSize: {PageSize}", 
                $"{BaseEndpoint}/load", page, pageSize);

            var result = await _httpClient.GetAsync<List<ProductDTO>>($"{BaseEndpoint}/load?page={page}&pageSize={pageSize}");
            
            if (result == null)
            {
                _logger.LogWarning("API returned null result");
                return new List<ProductDTO>();
            }
            
            _logger.LogInformation("Successfully retrieved {Count} products", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all products. Full exception details: {ExceptionDetails}", 
                ex.ToString());
            throw new Exception($"Failed to load products from API. Base URL: {_configuration["ApiSettings:BaseUrl"]}", ex);
        }
    }

    public async Task<List<ProductDTO>> SearchProductsAsync(
        string? name = null, string? category = null, 
        string? size = null, string? color = null, 
        string? variantId = null, decimal? price = null, 
        int page = 1, int pageSize = 10)
    {
        try
        {
            var queryParams = new List<string>();
            
            // Only add search parameters if they are provided
            if (!string.IsNullOrEmpty(name)) queryParams.Add($"searchTerm={Uri.EscapeDataString(name)}");
            if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={Uri.EscapeDataString(category)}");
            
            // Add pagination parameters
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = string.Join("&", queryParams);
            var endpoint = $"{BaseEndpoint}/load?{queryString}";
            
            _logger.LogInformation("Calling API to load products with filters: {Endpoint}", endpoint);
            var result = await _httpClient.GetAsync<List<ProductDTO>>(endpoint);
            
            if (result == null)
            {
                _logger.LogWarning("API returned null result for filtered load");
                return new List<ProductDTO>();
            }
            
            _logger.LogInformation("Successfully retrieved {Count} products with filters", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading filtered products");
            throw new Exception("Failed to load filtered products from API", ex);
        }
    }
} 