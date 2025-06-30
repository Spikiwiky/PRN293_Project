using EcommerceFrontend.Web.Extensions;
using EcommerceFrontend.Web.Models.Sale;
using EcommerceFrontend.Web.Services.Admin;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EcommerceFrontend.Web.Services.Sale
{
    public class SaleProductService : ISaleProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SaleProductService> _logger;
        private const string BaseEndpoint = "api/admin/products";

        public SaleProductService(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<SaleProductService> logger)
        {
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API base URL not configured"));
            _logger = logger;
        }

        public async Task<List<SaleProductDto>> GetAllProductsAsync(int page = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync($"{BaseEndpoint}?page={page}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<SaleProductDto>>() ?? new List<SaleProductDto>();
        }

        public async Task<SaleProductDto?> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseEndpoint}/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SaleProductDto>();
        }

        public async Task<List<SaleProductDto>> SearchProductsAsync(
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
                if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value.ToString("yyyy-MM-dd")}");
                if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value.ToString("yyyy-MM-dd")}");
                if (isFeatured.HasValue) queryParams.Add($"isFeatured={isFeatured}");

                var queryString = string.Join("&", queryParams);
                _logger.LogInformation("Sending search request to API. Endpoint: {Endpoint}, Parameters: {Params}",
                    $"{BaseEndpoint}/search", queryString);

                var response = await _httpClient.GetAsync($"{BaseEndpoint}/search?{queryString}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("No products found matching the search criteria");
                    return new List<SaleProductDto>();
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API returned error {StatusCode}: {Error}",
                        response.StatusCode, errorContent);

                    var errorMessage = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.BadRequest => $"Invalid search parameters: {errorContent}",
                        System.Net.HttpStatusCode.Unauthorized => "Authentication required to search products",
                        System.Net.HttpStatusCode.Forbidden => "You don't have permission to search products",
                        System.Net.HttpStatusCode.InternalServerError => "An error occurred on the server while searching products",
                        _ => $"Error searching products: {response.StatusCode}"
                    };

                    throw new HttpRequestException(errorMessage, null, response.StatusCode);
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var products = await response.Content.ReadFromJsonAsync<List<SaleProductDto>>(options);
                if (products == null)
                {
                    _logger.LogWarning("API returned null result");
                    return new List<SaleProductDto>();
                }

                _logger.LogInformation("Successfully retrieved {Count} products from API", products.Count);
                return products;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while searching products. Status code: {StatusCode}", ex.StatusCode);
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing product search results");
                throw new HttpRequestException("Invalid response format from the server", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with parameters: name={Name}, category={Category}, size={Size}, color={Color}, minPrice={MinPrice}, maxPrice={MaxPrice}, startDate={StartDate}, endDate={EndDate}, isFeatured={IsFeatured}, page={Page}, pageSize={PageSize}",
                    name, category, size, color, minPrice, maxPrice, startDate, endDate, isFeatured, page, pageSize);
                throw;
            }
        }

        public async Task<SaleProductDto> CreateProductAsync(SaleProductCreateDto createDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseEndpoint}", createDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SaleProductDto>()
                ?? throw new InvalidOperationException("Failed to deserialize the created product");
        }

        public async Task<SaleProductDto> UpdateProductAsync(SaleProductUpdateDto updateDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{updateDto.ProductId}", updateDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SaleProductDto>()
                ?? throw new InvalidOperationException("Failed to deserialize the updated product");
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete product with ID: {Id}", id);
                var response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");

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
                var response = await _httpClient.PatchAsJsonAsync($"{BaseEndpoint}/{id}/featured", isFeatured);
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
                var response = await _httpClient.PatchAsJsonAsync($"{BaseEndpoint}/{id}/status", status);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update product status: {ex.Message}", ex);
            }
        }

        public async Task<int> GetTotalProductCountAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseEndpoint}/count");
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

