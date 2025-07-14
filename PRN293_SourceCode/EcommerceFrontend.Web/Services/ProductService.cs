using System.Linq;
using System.Text;
using System.Text.Json;
using EcommerceFrontend.Web.Models;
using Microsoft.Extensions.Logging;

namespace EcommerceFrontend.Web.Services
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllProductsAsync(int page = 1, int pageSize = 10);
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<List<ProductDTO>> SearchProductsAsync(ProductSearchParams searchParams);
        Task<int> GetTotalProductsCountAsync(string? name = null, string? category = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task<Dictionary<string, List<string>>> GetProductAttributesAsync(int productId);
        Task<bool> AddProductAttributeAsync(int productId, string attributeName, List<string> attributeValues);
        Task<bool> UpdateProductAttributeAsync(int productId, string attributeName, List<string> attributeValues);
        Task<bool> DeleteProductAttributeAsync(int productId, string attributeName);
        Task<bool> UpdateProductAvailableAttributesAsync(int productId, Dictionary<string, List<string>> availableAttributes);
        Task<bool> AddProductVariantAsync(ProductVariantDTO variant);
        Task<bool> UpdateProductVariantAsync(ProductVariantDTO variant);
        Task<bool> DeleteProductVariantAsync(int variantId);
        Task<List<Dictionary<string, string>>> GetVariantValuesAsync(int variantId);
        Task<bool> AddVariantValueAsync(int variantId, Dictionary<string, string> variantValue);
        Task<bool> UpdateVariantValueAsync(int variantId, int valueIndex, Dictionary<string, string> variantValue);
        Task<bool> DeleteVariantValueAsync(int variantId, int valueIndex);
        Task<bool> UpdateProductAsync(int id, ProductDTO product);
        Task<bool> CreateProductAsync(ProductDTO product);
        Task<bool> AddProductImageAsync(int productId, string imageUrl);
    }

    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductService> _logger;

        public ProductService(HttpClient httpClient, IConfiguration configuration, ILogger<ProductService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var apiUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
            _httpClient.BaseAddress = new Uri(apiUrl);
            _logger = logger;
        }

        public async Task<List<ProductDTO>> GetAllProductsAsync(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation($"minPrice={null}, maxPrice={null}, page={page}, pageSize={pageSize}");
            var response = await _httpClient.GetAsync($"/api/product?page={page}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductDTO>>(content, _jsonOptions) ?? new List<ProductDTO>();
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/product/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductDTO>(content, _jsonOptions);
        }

        public async Task<List<ProductDTO>> SearchProductsAsync(ProductSearchParams searchParams)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(searchParams.Name))
                queryParams.Add($"name={Uri.EscapeDataString(searchParams.Name)}");
            if (!string.IsNullOrEmpty(searchParams.Category))
                queryParams.Add($"category={Uri.EscapeDataString(searchParams.Category)}");
            if (searchParams.Attributes != null && searchParams.Attributes.Any())
                queryParams.Add($"attributes={Uri.EscapeDataString(JsonSerializer.Serialize(searchParams.Attributes))}");
            if (searchParams.MinPrice.HasValue)
                queryParams.Add($"minPrice={searchParams.MinPrice}");
            if (searchParams.MaxPrice.HasValue)
                queryParams.Add($"maxPrice={searchParams.MaxPrice}");
            if (searchParams.Page.HasValue)
                queryParams.Add($"page={searchParams.Page}");
            if (searchParams.PageSize.HasValue)
                queryParams.Add($"pageSize={searchParams.PageSize}");

            var queryString = string.Join("&", queryParams);
            _logger.LogInformation($"name={searchParams.Name}, category={searchParams.Category}, minPrice={searchParams.MinPrice}, maxPrice={searchParams.MaxPrice}, page={searchParams.Page}, pageSize={searchParams.PageSize}");
            var response = await _httpClient.GetAsync($"/api/product/search?{queryString}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<ProductDTO>>(content, _jsonOptions) ?? new List<ProductDTO>();

            if (searchParams.Attributes != null && searchParams.Attributes.Any())
            {
                products = products.Where(p => p.Variants.Any(variant =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(variant.Attributes)) return false;
                        var variantAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(variant.Attributes);
                        return searchParams.Attributes.All(attr =>
                            variantAttributes.TryGetValue(attr.Key, out var value) && value == attr.Value);
                    }
                    catch
                    {
                        return false;
                    }
                })).ToList();
            }

            if (searchParams.Page < 1) searchParams.Page = 1;
            if (searchParams.PageSize <= 0) searchParams.PageSize = 10;

            products = products
                .OrderByDescending(p => p.ProductId)
                .Skip((int)((searchParams.Page - 1) * searchParams.PageSize))
                .Take((int)searchParams.PageSize)
                .ToList();

            return products;
        }

        public async Task<int> GetTotalProductsCountAsync(string? name = null, string? category = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(name))
                queryParams.Add($"name={Uri.EscapeDataString(name)}");
            if (!string.IsNullOrEmpty(category))
                queryParams.Add($"category={Uri.EscapeDataString(category)}");
            if (minPrice.HasValue)
                queryParams.Add($"minPrice={minPrice}");
            if (maxPrice.HasValue)
                queryParams.Add($"maxPrice={maxPrice}");

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"/api/product/count?{queryString}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<int>(content, _jsonOptions);
        }

        public async Task<Dictionary<string, List<string>>> GetProductAttributesAsync(int productId)
        {
            var response = await _httpClient.GetAsync($"/api/product/{productId}/attributes");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(content, _jsonOptions) ?? new Dictionary<string, List<string>>();
        }

        public async Task<bool> AddProductAttributeAsync(int productId, string attributeName, List<string> attributeValues)
        {
            var request = new { attributeName, attributeValues };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/api/product/{productId}/attributes", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseContent, _jsonOptions);
        }

        public async Task<bool> UpdateProductAttributeAsync(int productId, string attributeName, List<string> attributeValues)
        {
            var content = new StringContent(JsonSerializer.Serialize(attributeValues), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/product/{productId}/attributes/{attributeName}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseContent, _jsonOptions);
        }

        public async Task<bool> DeleteProductAttributeAsync(int productId, string attributeName)
        {
            var response = await _httpClient.DeleteAsync($"/api/product/{productId}/attributes/{attributeName}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(content, _jsonOptions);
        }

        public async Task<bool> UpdateProductAvailableAttributesAsync(int productId, Dictionary<string, List<string>> availableAttributes)
        {
            var content = new StringContent(JsonSerializer.Serialize(availableAttributes), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/product/{productId}/available-attributes", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseContent, _jsonOptions);
        }

        public async Task<bool> AddProductVariantAsync(ProductVariantDTO variant)
        {
            var content = new StringContent(JsonSerializer.Serialize(variant), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/product/variants", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseContent, _jsonOptions);
        }

        public async Task<bool> UpdateProductVariantAsync(ProductVariantDTO variant)
        {
            var content = new StringContent(JsonSerializer.Serialize(variant), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("/api/product/variants", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseContent, _jsonOptions);
        }

        public async Task<bool> DeleteProductVariantAsync(int variantId)
        {
            var response = await _httpClient.DeleteAsync($"/api/product/variants/{variantId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(content, _jsonOptions);
        }

        public async Task<List<Dictionary<string, string>>> GetVariantValuesAsync(int variantId)
        {
            var response = await _httpClient.GetAsync($"/api/product/variants/{variantId}/values");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Dictionary<string, string>>>(content, _jsonOptions) ?? new List<Dictionary<string, string>>();
        }

        public async Task<bool> AddVariantValueAsync(int variantId, Dictionary<string, string> variantValue)
        {
            var content = new StringContent(JsonSerializer.Serialize(variantValue), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/api/product/variants/{variantId}/values", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseContent, _jsonOptions);
        }

        public async Task<bool> UpdateVariantValueAsync(int variantId, int valueIndex, Dictionary<string, string> variantValue)
        {
            var content = new StringContent(JsonSerializer.Serialize(variantValue), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/product/variants/{variantId}/values/{valueIndex}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseContent, _jsonOptions);
        }

        public async Task<bool> DeleteVariantValueAsync(int variantId, int valueIndex)
        {
            var response = await _httpClient.DeleteAsync($"/api/product/variants/{variantId}/values/{valueIndex}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(content, _jsonOptions);
        }

        public async Task<bool> UpdateProductAsync(int id, ProductDTO product)
        {
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/admin/products/{id}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseContent, _jsonOptions);
        }

        public async Task<bool> CreateProductAsync(ProductDTO product)
        {
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/api/admin/products", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseContent, _jsonOptions);
        }

        public async Task<bool> AddProductImageAsync(int productId, string imageUrl)
        {
            var request = new { imageUrl };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/api/product/{productId}/images", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseContent, _jsonOptions);
        }
    }
} 