using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Models.Sale;
using EcommerceFrontend.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace EcommerceFrontend.Web.Pages.Cart;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiSettings _apiSettings;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ILogger<IndexModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _apiSettings = apiSettings.Value ?? throw new ArgumentNullException(nameof(apiSettings), "ApiSettings is not configured.");
        _logger = logger;
    }

    public List<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
    public int CartItemCount { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            // TODO: Get current user ID from authentication
            var userId = 1; // Placeholder - should get from JWT token

            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.GetAsync($"{_apiSettings.BaseUrl}/api/cart");

            if (response.IsSuccessStatusCode)
            {
                var cartResponse = await response.Content.ReadFromJsonAsync<CartResponseDto>();
                if (cartResponse?.Success == true && cartResponse.Cart != null)
                {
                    CartItems = cartResponse.Cart.CartDetails?.Select(cd => new CartItemModel
                    {
                        CartDetailId = cd.CartDetailId,
                        ProductId = cd.ProductId,
                        ProductName = cd.ProductName,
                        ProductBrand = cd.ProductBrand,
                        ProductImageUrl = cd.ProductImageUrl,
                        VariantAttributes = cd.VariantAttributes,
                        Quantity = cd.Quantity,
                        Price = cd.Price
                    }).ToList() ?? new List<CartItemModel>();

                    CartItemCount = cartResponse.Summary?.CartItemCount ?? 0;
                    TotalQuantity = cartResponse.Summary?.TotalItems ?? 0;
                    TotalAmount = cartResponse.Summary?.TotalAmount ?? 0;
                }
            }
            else
            {
                _logger.LogWarning("Failed to get cart. Status: {StatusCode}", response.StatusCode);
                ErrorMessage = "Không thể tải giỏ hàng. Vui lòng thử lại.";
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading cart");
            ErrorMessage = "Có lỗi xảy ra khi tải giỏ hàng. Vui lòng thử lại.";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostUpdateQuantityAsync([FromBody] UpdateQuantityRequest request)
    {
        try
        {
            if (request.Quantity <= 0)
            {
                return new JsonResult(new { success = false, message = "Số lượng phải lớn hơn 0" });
            }

            var client = _httpClientFactory.CreateClient("MyAPI");
            var updateRequest = new
            {
                cartDetailId = request.CartDetailId,
                quantity = request.Quantity
            };

            var json = JsonSerializer.Serialize(updateRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_apiSettings.BaseUrl}/api/cart/update", content);

            if (response.IsSuccessStatusCode)
            {
                return new JsonResult(new { success = true, message = "Cập nhật số lượng thành công" });
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return new JsonResult(new { success = false, message = error });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart item quantity");
            return new JsonResult(new { success = false, message = "Có lỗi xảy ra khi cập nhật số lượng" });
        }
    }

    public async Task<IActionResult> OnPostRemoveFromCartAsync([FromBody] RemoveFromCartRequest request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var removeRequest = new
            {
                cartDetailId = request.CartDetailId
            };

            var json = JsonSerializer.Serialize(removeRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.DeleteAsync($"{_apiSettings.BaseUrl}/api/cart/remove");

            if (response.IsSuccessStatusCode)
            {
                return new JsonResult(new { success = true, message = "Xóa sản phẩm khỏi giỏ hàng thành công" });
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return new JsonResult(new { success = false, message = error });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from cart");
            return new JsonResult(new { success = false, message = "Có lỗi xảy ra khi xóa sản phẩm" });
        }
    }

    public async Task<IActionResult> OnPostClearCartAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.DeleteAsync($"{_apiSettings.BaseUrl}/api/cart/clear");

            if (response.IsSuccessStatusCode)
            {
                return new JsonResult(new { success = true, message = "Xóa giỏ hàng thành công" });
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return new JsonResult(new { success = false, message = error });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
            return new JsonResult(new { success = false, message = "Có lỗi xảy ra khi xóa giỏ hàng" });
        }
    }

    public async Task<IActionResult> OnGetGetCartCountAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.GetAsync($"{_apiSettings.BaseUrl}/api/cart/summary");

            if (response.IsSuccessStatusCode)
            {
                var summary = await response.Content.ReadFromJsonAsync<CartSummaryDto>();
                return Content((summary?.CartItemCount ?? 0).ToString());
            }

            return Content("0");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart count");
            return Content("0");
        }
    }
}

// Models
public class CartItemModel
{
    public int CartDetailId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductBrand { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public string? VariantAttributes { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class UpdateQuantityRequest
{
    public int CartDetailId { get; set; }
    public int Quantity { get; set; }
}

public class RemoveFromCartRequest
{
    public int CartDetailId { get; set; }
}

// API Response Models
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