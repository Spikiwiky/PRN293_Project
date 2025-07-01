using EcommerceFrontend.Web.Models.Order;
using System.Text.Json;

namespace EcommerceFrontend.Web.Services.Order
{
    // Interface
    public interface IOrderService
    {
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<bool> IncreaseQuantityAsync(int orderId, int productId, string? variantId);
        Task<bool> DecreaseQuantityAsync(int orderId, int productId, string? variantId);
    }

    // Implementation
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderService> _logger;

        public OrderService(HttpClient httpClient, IConfiguration configuration, ILogger<OrderService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            var apiUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
            _httpClient.BaseAddress = new Uri(apiUrl);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var response = await _httpClient.GetAsync($"/api/Order/{orderId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Order not found: {OrderId}", orderId);
                return null;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderDto>(content, _jsonOptions);
        }

        public async Task<bool> IncreaseQuantityAsync(int orderId, int productId, string? variantId)
        {
            var response = await _httpClient.PostAsync($"/api/Order/{orderId}/increase?productId={productId}&variantId={variantId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DecreaseQuantityAsync(int orderId, int productId, string? variantId)
        {
            var response = await _httpClient.PostAsync($"/api/Order/{orderId}/decrease?productId={productId}&variantId={variantId}", null);
            return response.IsSuccessStatusCode;
        }

    }
}
