using EcommerceFrontend.Web.Models.DTOs;
using System.Text.Json;

namespace EcommerceFrontend.Web.Services;

public interface IProductService
{
    Task<List<ProductDTO>> GetAllProductsAsync(int page = 1, int pageSize = 10);
    Task<List<ProductDTO>> SearchProductsAsync(
        string? name = null, 
        string? category = null, 
        string? size = null, 
        string? color = null, 
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int page = 1, 
        int pageSize = 10);
    Task<ProductVariant> AddVariantAsync(AddVariantDTO variantDto);
    Task<ProductDTO?> GetProductByIdAsync(int productId);
    Task<List<ProductVariant>> GetProductVariantsAsync(int productId);
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
                $"{BaseEndpoint}", page, pageSize);

            var result = await _httpClient.GetAsync<List<ProductDTO>>($"{BaseEndpoint}?page={page}&pageSize={pageSize}");
            
            if (result == null)
            {
                _logger.LogWarning("API returned null result");
                return new List<ProductDTO>();
            }

            // Ensure variants are properly initialized
            foreach (var product in result)
            {
                product.Variants ??= new List<ProductVariant>();
                if (!product.Variants.Any() && product.Price.HasValue)
                {
                    // Create a default variant from legacy fields if no variants exist
                    product.Variants.Add(new ProductVariant
                    {
                        Size = product.Size,
                        Color = product.Color,
                        Categories = product.Category,
                        Price = product.Price.Value,
                        VariantId = product.VariantId
                    });
                }
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
        string? name = null, 
        string? category = null, 
        string? size = null, 
        string? color = null, 
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int page = 1, 
        int pageSize = 10)
    {
        try
        {
            var queryParams = new List<string>();
            
            if (!string.IsNullOrEmpty(name)) queryParams.Add($"name={Uri.EscapeDataString(name)}");
            if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={Uri.EscapeDataString(category)}");
            if (!string.IsNullOrEmpty(size)) queryParams.Add($"size={Uri.EscapeDataString(size)}");
            if (!string.IsNullOrEmpty(color)) queryParams.Add($"color={Uri.EscapeDataString(color)}");
            if (minPrice.HasValue) queryParams.Add($"minPrice={minPrice}");
            if (maxPrice.HasValue) queryParams.Add($"maxPrice={maxPrice}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = string.Join("&", queryParams);
            _logger.LogInformation("Searching products with query string: {QueryString}", queryString);

            var result = await _httpClient.GetAsync<List<ProductDTO>>($"{BaseEndpoint}/search?{queryString}");
            
            if (result == null)
            {
                _logger.LogWarning("API returned null result for search");
                return new List<ProductDTO>();
            }

            // Ensure variants are properly initialized
            foreach (var product in result)
            {
                product.Variants ??= new List<ProductVariant>();
                if (!product.Variants.Any() && product.Price.HasValue)
                {
                    // Create a default variant from legacy fields if no variants exist
                    product.Variants.Add(new ProductVariant
                    {
                        Size = product.Size,
                        Color = product.Color,
                        Categories = product.Category,
                        Price = product.Price.Value,
                        VariantId = product.VariantId
                    });
                }
            }
            
            _logger.LogInformation("Successfully found {Count} products", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products. Full exception details: {ExceptionDetails}", 
                ex.ToString());
            throw new Exception($"Failed to search products from API. Base URL: {_configuration["ApiSettings:BaseUrl"]}", ex);
        }
    }

    public async Task<ProductVariant> AddVariantAsync(AddVariantDTO variantDto)
    {
        try
        {
            _logger.LogInformation("Adding new variant for product {ProductId}", variantDto.ProductId);
            
            var result = await _httpClient.PostAsync<ProductVariant>($"{BaseEndpoint}/variants", variantDto);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding variant for product {ProductId}. Full exception details: {ExceptionDetails}", 
                variantDto.ProductId, ex.ToString());
            throw new Exception($"Failed to add variant. Base URL: {_configuration["ApiSettings:BaseUrl"]}", ex);
        }
    }

    public async Task<ProductDTO?> GetProductByIdAsync(int productId)
    {
        try
        {
            _logger.LogInformation("Getting product by ID: {ProductId}", productId);
            
            var result = await _httpClient.GetAsync<ProductDTO>($"{BaseEndpoint}/{productId}");
            
            if (result == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", productId);
                return null;
            }

            // Ensure variants are properly initialized
            result.Variants ??= new List<ProductVariant>();
            if (!result.Variants.Any() && result.Price.HasValue)
            {
                // Create a default variant from legacy fields if no variants exist
                result.Variants.Add(new ProductVariant
                {
                    Size = result.Size,
                    Color = result.Color,
                    Categories = result.Category,
                    Price = result.Price.Value,
                    VariantId = result.VariantId
                });
            }
            
            _logger.LogInformation("Successfully retrieved product {ProductId}", productId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}. Full exception details: {ExceptionDetails}", 
                productId, ex.ToString());
            throw new Exception($"Failed to load product from API. Base URL: {_configuration["ApiSettings:BaseUrl"]}", ex);
        }
    }

    public async Task<List<ProductVariant>> GetProductVariantsAsync(int productId)
    {
        try
        {
            _logger.LogInformation("Getting variants for product {ProductId}", productId);
            
            var result = await _httpClient.GetAsync<List<ProductVariant>>($"{BaseEndpoint}/{productId}/variants");
            
            if (result == null)
            {
                _logger.LogWarning("No variants found for product {ProductId}", productId);
                return new List<ProductVariant>();
            }
            
            _logger.LogInformation("Successfully retrieved {Count} variants for product {ProductId}", 
                result.Count, productId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting variants for product {ProductId}. Full exception details: {ExceptionDetails}", 
                productId, ex.ToString());
            throw new Exception($"Failed to load variants from API. Base URL: {_configuration["ApiSettings:BaseUrl"]}", ex);
        }
    }
} 