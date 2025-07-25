using EcommerceFrontend.Web.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace EcommerceFrontend.Web.Services;

public interface ICartService
{
    Task<bool> AddToCartAsync(AddToCartRequest request);
    Task<CartResponseDto?> GetCartAsync();
    Task<bool> UpdateCartItemQuantityAsync(int cartDetailId, int quantity);
    Task<bool> RemoveFromCartAsync(int cartDetailId);
    Task<bool> ClearCartAsync();
    Task<CartSummaryDto?> GetCartSummaryAsync();
}

public class CartService : ICartService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiSettings _apiSettings;

    public CartService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _apiSettings = apiSettings.Value;
    }

    public async Task<bool> AddToCartAsync(AddToCartRequest request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_apiSettings.BaseUrl}/api/cart/add", content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<CartResponseDto?> GetCartAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.GetAsync($"{_apiSettings.BaseUrl}/api/cart");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CartResponseDto>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateCartItemQuantityAsync(int cartDetailId, int quantity)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var request = new { cartDetailId, quantity };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_apiSettings.BaseUrl}/api/cart/update", content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveFromCartAsync(int cartDetailId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var request = new { cartDetailId };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.DeleteAsync($"{_apiSettings.BaseUrl}/api/cart/remove");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ClearCartAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.DeleteAsync($"{_apiSettings.BaseUrl}/api/cart/clear");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<CartSummaryDto?> GetCartSummaryAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.GetAsync($"{_apiSettings.BaseUrl}/api/cart/summary");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CartSummaryDto>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
}

public class AddToCartRequest
{
    public int ProductId { get; set; }
    public string? VariantId { get; set; }
    public string? VariantAttributes { get; set; }
    public int Quantity { get; set; } = 1;
}

public class CartResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public CartDto? Cart { get; set; }
    public CartSummaryDto? Summary { get; set; }
}

public class CartDto
{
    public int CartId { get; set; }
    public int CustomerId { get; set; }
    public int TotalQuantity { get; set; }
    public decimal AmountDue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CartDetailDto>? CartDetails { get; set; }
}

public class CartDetailDto
{
    public int CartDetailId { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductBrand { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public string? VariantId { get; set; }
    public string? VariantAttributes { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class CartSummaryDto
{
    public int TotalItems { get; set; }
    public decimal TotalAmount { get; set; }
    public int CartItemCount { get; set; }
} 