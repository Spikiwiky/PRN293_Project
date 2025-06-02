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
            
            // Add all search parameters that match the backend API parameters
            if (!string.IsNullOrEmpty(name)) queryParams.Add($"name={Uri.EscapeDataString(name)}");
            if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={Uri.EscapeDataString(category)}");
            if (!string.IsNullOrEmpty(size)) queryParams.Add($"size={Uri.EscapeDataString(size)}");
            if (!string.IsNullOrEmpty(color)) queryParams.Add($"color={Uri.EscapeDataString(color)}");
            if (!string.IsNullOrEmpty(variantId)) queryParams.Add($"variantId={Uri.EscapeDataString(variantId)}");
            if (price.HasValue) queryParams.Add($"price={price}");
            
            // Add pagination parameters
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = string.Join("&", queryParams);
            var endpoint = $"{BaseEndpoint}/search?{queryString}";
            
            _logger.LogInformation("Calling API to search products: {Endpoint}", endpoint);
            var result = await _httpClient.GetAsync<List<ProductDTO>>(endpoint);
            
            if (result == null)
            {
                _logger.LogWarning("API returned null result for search");
                return new List<ProductDTO>();
            }
            
            _logger.LogInformation("Successfully retrieved {Count} products from search", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products");
            throw new Exception("Failed to search products from API", ex);
        }
    }
} 