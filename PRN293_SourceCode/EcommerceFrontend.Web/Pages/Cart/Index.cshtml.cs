using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace EcommerceFrontend.Web.Pages.Cart;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public List<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
    public int CartItemCount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? ErrorMessage { get; set; }

    private string GetProductImageUrl(string? productImage)
    {
        if (string.IsNullOrEmpty(productImage))
            return "/images/default-product.jpg";
        
        // If it's already a full URL, return as is
        if (productImage.StartsWith("http"))
            return productImage;
        
        // If it's a relative path starting with /images/, convert to backend URL
        if (productImage.StartsWith("/images/"))
        {
            var backendUrl = "https://localhost:7257"; // Backend API URL
            return $"{backendUrl}{productImage}";
        }
        
        // Default fallback
        return "/images/default-product.jpg";
    }

    [HttpGet("test-cookies")]
    public IActionResult TestCookies()
    {
        var allCookies = Request.Cookies.Select(c => new { Key = c.Key, Value = c.Value }).ToList();
        var userId = Request.Cookies["UserId"];
        
        return new JsonResult(new
        {
            success = true,
            cookieCount = allCookies.Count,
            allCookies = allCookies,
            userId = userId,
            hasUserId = !string.IsNullOrEmpty(userId)
        });
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            _logger.LogInformation("Cart page OnGetAsync called");
            
            // Debug: Log all cookies
            var allCookies = Request.Cookies.Select(c => new { Key = c.Key, Value = c.Value }).ToList();
            _logger.LogInformation("All cookies in Cart page: {Cookies}", string.Join(", ", allCookies.Select(c => $"{c.Key}={c.Value}")));
            
            // Get userId from cookies
            var userIdStr = Request.Cookies["UserId"];
            if (string.IsNullOrEmpty(userIdStr))
            {
                _logger.LogWarning("No UserId found in cookies");
                ErrorMessage = "Vui lòng đăng nhập để xem giỏ hàng";
                CartItems = new List<CartItemModel>();
                return Page();
            }

            if (!int.TryParse(userIdStr, out int userId))
            {
                _logger.LogWarning("Invalid UserId in cookies: {UserIdStr}", userIdStr);
                ErrorMessage = "Thông tin người dùng không hợp lệ";
                CartItems = new List<CartItemModel>();
                return Page();
            }

            _logger.LogInformation("Loading cart for user ID: {UserId}", userId);

            // Call frontend CartController to get cart data (backend will get userId from cookies)
            var client = _httpClientFactory.CreateClient();
            var cartUrl = $"{Request.Scheme}://{Request.Host}/api/Cart";
            
            _logger.LogInformation("Calling frontend CartController: {CartUrl}", cartUrl);
            
            // Create request with cookies
            var request = new HttpRequestMessage(HttpMethod.Get, cartUrl);
            
            // Forward all cookies from the current request
            var cookieHeader = Request.Headers["Cookie"].ToString();
            if (!string.IsNullOrEmpty(cookieHeader))
            {
                request.Headers.Add("Cookie", cookieHeader);
                _logger.LogInformation("Forwarding cookies: {CookieHeader}", cookieHeader);
            }
            else
            {
                _logger.LogWarning("No cookies found in request headers");
            }
            
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("CartController response status: {StatusCode}", response.StatusCode);
            _logger.LogInformation("CartController response content: {Content}", content);
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    _logger.LogInformation("Starting JSON parsing...");
                    
                    // Parse the outer response first
                    var outerResponse = JsonSerializer.Deserialize<JsonElement>(content);
                    _logger.LogInformation("Outer response parsed successfully");
                    
                    if (outerResponse.TryGetProperty("data", out var dataElement))
                    {
                        _logger.LogInformation("Found 'data' property, parsing inner object...");
                        
                        // Parse the inner data object
                        var cartResponse = JsonSerializer.Deserialize<CartResponseDto>(dataElement.GetRawText());
                        _logger.LogInformation("Inner cartResponse parsed successfully");
                        
                        _logger.LogInformation("Parsed cartResponse - Success: {Success}, Cart: {Cart}, CartDetails: {CartDetails}", 
                            cartResponse?.Success, 
                            cartResponse?.Cart != null ? "Not null" : "null",
                            cartResponse?.Cart?.CartDetails?.Count ?? 0);
                        
                        if (cartResponse?.Success == true && cartResponse.Cart != null)
                        {
                            // Parse cart details
                            if (cartResponse.Cart.CartDetails != null)
                            {
                                CartItems = cartResponse.Cart.CartDetails.Select(cd => new CartItemModel
                                {
                                    CartDetailId = cd.CartDetailId,
                                    ProductId = cd.ProductId,
                                    ProductName = cd.ProductName,
                                    ProductImage = GetProductImageUrl(cd.ProductImage),
                                    VariantId = cd.VariantId,
                                    VariantAttributes = cd.VariantAttributes,
                                    Quantity = cd.Quantity,
                                    Price = cd.Price
                                }).ToList();
                            }
                            else
                            {
                                CartItems = new List<CartItemModel>();
                            }
                            
                            CartItemCount = CartItems.Count;
                            TotalAmount = CartItems.Sum(item => item.Price * item.Quantity);
                            
                            _logger.LogInformation("Successfully loaded {Count} cart items", CartItems.Count);
                        }
                        else
                        {
                            CartItems = new List<CartItemModel>();
                            CartItemCount = 0;
                            TotalAmount = 0;
                            _logger.LogInformation("Cart is empty or response indicates no items");
                        }
                    }
                    else
                    {
                        _logger.LogError("No 'data' property found in response");
                        ErrorMessage = "Dữ liệu giỏ hàng không hợp lệ";
                        CartItems = new List<CartItemModel>();
                    }
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Failed to parse cart response");
                    ErrorMessage = "Lỗi khi xử lý dữ liệu giỏ hàng";
                    CartItems = new List<CartItemModel>();
                }
            }
            else
            {
                _logger.LogError("CartController returned error: {StatusCode}, {Content}", response.StatusCode, content);
                ErrorMessage = $"Lỗi khi tải giỏ hàng: {response.StatusCode}";
                CartItems = new List<CartItemModel>();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading cart");
            ErrorMessage = "Có lỗi xảy ra khi tải giỏ hàng. Vui lòng thử lại.";
            CartItems = new List<CartItemModel>();
            return Page();
        }
    }
}

// Models
public class CartItemModel
{
    public int CartDetailId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public int? VariantId { get; set; }
    public string? VariantAttributes { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

// API Response Models
public class CartResponseDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("cart")]
    public CartDto? Cart { get; set; }
    
    [JsonPropertyName("summary")]
    public CartSummaryDto? Summary { get; set; }
}

public class CartDto
{
    [JsonPropertyName("cartId")]
    public int CartId { get; set; }
    
    [JsonPropertyName("customerId")]
    public int CustomerId { get; set; }
    
    [JsonPropertyName("totalQuantity")]
    public int TotalQuantity { get; set; }
    
    [JsonPropertyName("amountDue")]
    public decimal AmountDue { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
    
    [JsonPropertyName("cartDetails")]
    public List<CartDetailDto>? CartDetails { get; set; }
}

public class CartDetailDto
{
    [JsonPropertyName("cartDetailId")]
    public int CartDetailId { get; set; }
    
    [JsonPropertyName("cartId")]
    public int CartId { get; set; }
    
    [JsonPropertyName("productId")]
    public int ProductId { get; set; }
    
    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;
    
    [JsonPropertyName("variantId")]
    public int? VariantId { get; set; }
    
    [JsonPropertyName("variantAttributes")]
    public string? VariantAttributes { get; set; }
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("productImage")]
    public string? ProductImage { get; set; }
    
    [JsonPropertyName("productDescription")]
    public string? ProductDescription { get; set; }
}

public class CartSummaryDto
{
    [JsonPropertyName("totalItems")]
    public int TotalItems { get; set; }
    
    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }
    
    [JsonPropertyName("cartItemCount")]
    public int CartItemCount { get; set; }
} 