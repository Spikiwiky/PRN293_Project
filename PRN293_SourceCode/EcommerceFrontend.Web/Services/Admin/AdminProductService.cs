using System.Net.Http.Json;
using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Extensions;
using Microsoft.Extensions.Logging;

namespace EcommerceFrontend.Web.Services.Admin
{
    public class AdminProductService : IAdminProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AdminProductService> _logger;

        public AdminProductService(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<AdminProductService> logger)
        {
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API base URL not configured"));
            _logger = logger;
        }

        public async Task<List<AdminProductDto>> GetAllProductsAsync(int page = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync($"api/admin/products?page={page}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<AdminProductDto>>() ?? new List<AdminProductDto>();
        }

        public async Task<AdminProductDto?> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/admin/products/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AdminProductDto>();
        }

        public async Task<List<AdminProductDto>> SearchProductsAsync(
            string? name = null,
            string? category = null,
            string? size = null,
            string? color = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? isFeatured = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrEmpty(name)) queryParams.Add($"name={Uri.EscapeDataString(name)}");
                if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={Uri.EscapeDataString(category)}");
                if (!string.IsNullOrEmpty(size)) queryParams.Add($"size={Uri.EscapeDataString(size)}");
                if (!string.IsNullOrEmpty(color)) queryParams.Add($"color={Uri.EscapeDataString(color)}");
                if (minPrice.HasValue) queryParams.Add($"minPrice={minPrice}");
                if (maxPrice.HasValue) queryParams.Add($"maxPrice={maxPrice}");
                if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
                if (isFeatured.HasValue) queryParams.Add($"isFeatured={isFeatured}");

                _logger.LogInformation("Sending search request to API with parameters: {Params}", string.Join("&", queryParams));
                var response = await _httpClient.GetAsync($"api/admin/products/search?{string.Join("&", queryParams)}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API returned error {StatusCode}: {Error}", 
                        response.StatusCode, errorContent);
                    throw new HttpRequestException($"API returned {response.StatusCode}: {errorContent}");
                }

                var products = await response.Content.ReadFromJsonAsync<List<AdminProductDto>>();
                _logger.LogInformation("Successfully retrieved {Count} products from API", products?.Count ?? 0);
                return products ?? new List<AdminProductDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with parameters: name={Name}, category={Category}, size={Size}, color={Color}, minPrice={MinPrice}, maxPrice={MaxPrice}, startDate={StartDate}, endDate={EndDate}, isFeatured={IsFeatured}, page={Page}, pageSize={PageSize}",
                    name, category, size, color, minPrice, maxPrice, startDate, endDate, isFeatured, page, pageSize);
                throw;
            }
        }

        public async Task<AdminProductDto> CreateProductAsync(AdminProductCreateDto createDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/products", createDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AdminProductDto>() 
                ?? throw new InvalidOperationException("Failed to deserialize the created product");
        }

        public async Task<AdminProductDto> UpdateProductAsync(AdminProductUpdateDto updateDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/admin/products/{updateDto.ProductId}", updateDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AdminProductDto>()
                ?? throw new InvalidOperationException("Failed to deserialize the updated product");
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete product with ID: {Id}", id);
                var response = await _httpClient.DeleteAsync($"api/admin/products/{id}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Product with ID {Id} not found", id);
                    return false;
                }
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to delete product {Id}. Status: {Status}, Error: {Error}", 
                        id, response.StatusCode, errorContent);
                    throw new HttpRequestException($"Failed to delete product: {errorContent}", null, response.StatusCode);
                }

                _logger.LogInformation("Successfully deleted product with ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateProductFeaturedStatusAsync(int id, bool isFeatured)
        {
            try
            {
                var response = await _httpClient.PatchAsJsonAsync($"api/admin/products/{id}/featured", isFeatured);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update product featured status: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateProductStatusAsync(int id, int status)
        {
            try
            {
                var response = await _httpClient.PatchAsJsonAsync($"api/admin/products/{id}/status", status);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update product status: {ex.Message}", ex);
            }
        }

        public async Task<int> GetTotalProductCountAsync()
        {
            var response = await _httpClient.GetAsync("api/admin/products/count");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("api/admin/categories");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new List<CategoryDto>();
        }
    }
} 