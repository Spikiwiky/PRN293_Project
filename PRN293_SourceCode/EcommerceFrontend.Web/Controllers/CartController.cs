using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace EcommerceFrontend.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CartController> _logger;

        public CartController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<CartController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("test-cookies")]
        public IActionResult TestCookies()
        {
            try
            {
                _logger.LogInformation("=== Testing cookies ===");
                
                var allCookies = Request.Cookies.Select(c => new { Key = c.Key, Value = c.Value }).ToList();
                var userId = GetUserIdFromCookies();
                
                // Debug logging
                Console.WriteLine($"TestCookies - Total cookies: {allCookies.Count}");
                foreach (var cookie in allCookies)
                {
                    Console.WriteLine($"Cookie: {cookie.Key} = {cookie.Value}");
                }
                
                return Ok(new
                {
                    success = true,
                    cookieCount = allCookies.Count,
                    allCookies = allCookies,
                    userId = userId,
                    hasUserId = userId.HasValue,
                    message = userId.HasValue ? "User ID found" : "No User ID found"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing cookies");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestBackendConnection()
        {
            try
            {
                _logger.LogInformation("=== Testing backend connection ===");
                
                var backendUrl = _configuration["BackendAPI:BaseUrl"];
                _logger.LogInformation("Backend URL from config: {BackendUrl}", backendUrl);
                
                var client = _httpClientFactory.CreateClient("BackendAPI");
                _logger.LogInformation("HttpClient BaseAddress: {BaseAddress}", client.BaseAddress);
                
                var testUrl = $"{backendUrl}/api/Cart/test";
                _logger.LogInformation("Testing URL: {TestUrl}", testUrl);
                
                var response = await client.GetAsync(testUrl);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Content: {Content}", content);
                
                return Ok(new 
                { 
                    success = response.IsSuccessStatusCode,
                    statusCode = (int)response.StatusCode,
                    content = content,
                    testUrl = testUrl,
                    backendUrl = backendUrl,
                    clientBaseAddress = client.BaseAddress?.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Backend test failed");
                return StatusCode(500, new 
                { 
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("test-simple")]
        public async Task<IActionResult> TestSimpleConnection()
        {
            try
            {
                _logger.LogInformation("=== Testing simple backend connection ===");
                
                var backendUrl = _configuration["BackendAPI:BaseUrl"];
                var testUrl = $"{backendUrl}/api/Cart/test";
                
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(testUrl);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Simple test - Status: {StatusCode}, Content: {Content}", response.StatusCode, content);
                
                return Ok(new 
                { 
                    success = response.IsSuccessStatusCode,
                    statusCode = (int)response.StatusCode,
                    content = content,
                    testUrl = testUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Simple backend test failed");
                return StatusCode(500, new 
                { 
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpGet("test-cart")]
        public async Task<IActionResult> TestCartWithUserId()
        {
            try
            {
                _logger.LogInformation("=== Testing cart with user ID ===");
                
                var userId = GetUserIdFromCookies();
                _logger.LogInformation("User ID from cookies: {UserId}", userId);
                
                if (userId == null)
                {
                    return Ok(new { success = false, message = "No user ID found in cookies" });
                }
                
                var backendUrl = _configuration["BackendAPI:BaseUrl"];
                var testUrl = $"{backendUrl}/api/Cart?userId={userId}";
                
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(testUrl);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Cart test - Status: {StatusCode}, Content: {Content}", response.StatusCode, content);
                
                return Ok(new 
                { 
                    success = response.IsSuccessStatusCode,
                    statusCode = (int)response.StatusCode,
                    content = content,
                    testUrl = testUrl,
                    userId = userId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cart test failed");
                return StatusCode(500, new 
                { 
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpGet("test-cart-simple")]
        public async Task<IActionResult> TestCartSimple()
        {
            try
            {
                _logger.LogInformation("=== Testing simple cart ===");
                
                // First, let's debug cookies
                var allCookies = Request.Cookies.Select(c => new { Key = c.Key, Value = c.Value }).ToList();
                _logger.LogInformation("All cookies: {Cookies}", string.Join(", ", allCookies.Select(c => $"{c.Key}={c.Value}")));
                
                var userId = GetUserIdFromCookies();
                _logger.LogInformation("User ID from cookies: {UserId}", userId);
                
                if (userId == null)
                {
                    return Ok(new { 
                        success = false, 
                        message = "No user ID found in cookies",
                        cookies = allCookies,
                        cookieCount = allCookies.Count
                    });
                }
                
                // Test backend connection first
                var backendUrl = _configuration["BackendAPI:BaseUrl"];
                var testUrl = $"{backendUrl}/api/Cart/test";
                
                using var httpClient = new HttpClient();
                var testResponse = await httpClient.GetAsync(testUrl);
                var testContent = await testResponse.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Backend test - Status: {StatusCode}, Content: {Content}", testResponse.StatusCode, testContent);
                
                if (!testResponse.IsSuccessStatusCode)
                {
                    return Ok(new { success = false, message = "Backend not accessible", backendStatus = testResponse.StatusCode });
                }
                
                // Now test cart
                var cartUrl = $"{backendUrl}/api/Cart?userId={userId}";
                var cartResponse = await httpClient.GetAsync(cartUrl);
                var cartContent = await cartResponse.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Cart response - Status: {StatusCode}, Content: {Content}", cartResponse.StatusCode, cartContent);
                
                return Ok(new 
                { 
                    success = cartResponse.IsSuccessStatusCode,
                    userId = userId,
                    cookies = allCookies,
                    backendTest = new { status = testResponse.StatusCode, content = testContent },
                    cartTest = new { status = cartResponse.StatusCode, content = cartContent }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Simple cart test failed");
                return StatusCode(500, new 
                { 
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpGet("debug-cookies")]
        public IActionResult DebugCookies()
        {
            try
            {
                _logger.LogInformation("=== Debugging cookies ===");
                
                // Get all cookies
                var allCookies = Request.Cookies.Select(c => new { Key = c.Key, Value = c.Value }).ToList();
                
                // Get specific cookies
                var token = Request.Cookies["Token"];
                var userId = Request.Cookies["UserId"];
                var userName = Request.Cookies["UserName"];
                var roleName = Request.Cookies["RoleName"];
                
                // Try different cookie names
                var userIdAlt = Request.Cookies["userId"];
                var tokenAlt = Request.Cookies["token"];
                
                // Log all cookies to console for debugging
                Console.WriteLine("=== ALL COOKIES ===");
                foreach (var cookie in allCookies)
                {
                    Console.WriteLine($"Cookie: {cookie.Key} = {cookie.Value}");
                }
                Console.WriteLine("=== END COOKIES ===");
                
                var debugInfo = new
                {
                    allCookies = allCookies,
                    specificCookies = new
                    {
                        token = token,
                        userId = userId,
                        userName = userName,
                        roleName = roleName
                    },
                    alternativeNames = new
                    {
                        userIdAlt = userIdAlt,
                        tokenAlt = tokenAlt
                    },
                    cookieCount = allCookies.Count,
                    hasToken = !string.IsNullOrEmpty(token),
                    hasUserId = !string.IsNullOrEmpty(userId),
                    parsedUserId = GetUserIdFromCookies(),
                    requestHeaders = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
                };
                
                _logger.LogInformation("Debug info: {DebugInfo}", JsonSerializer.Serialize(debugInfo, new JsonSerializerOptions { WriteIndented = true }));
                
                return Ok(new
                {
                    success = true,
                    debugInfo = debugInfo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error debugging cookies");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("auth-status")]
        public IActionResult CheckAuthStatus()
        {
            try
            {
                _logger.LogInformation("=== Checking authentication status ===");
                
                // Log all cookies
                var allCookies = Request.Cookies.Select(c => new { Key = c.Key, Value = c.Value, Length = c.Value?.Length ?? 0 }).ToList();
                _logger.LogInformation("All cookies: {Cookies}", string.Join(", ", allCookies.Select(c => $"{c.Key}({c.Length})")));
                
                var token = Request.Cookies["Token"];
                var userName = Request.Cookies["UserName"];
                var roleName = Request.Cookies["RoleName"];
                var userId = Request.Cookies["UserId"];
                
                var authInfo = new
                {
                    hasToken = !string.IsNullOrEmpty(token),
                    tokenLength = token?.Length ?? 0,
                    tokenStart = token?.Substring(0, Math.Min(10, token?.Length ?? 0)),
                    userName = userName,
                    roleName = roleName,
                    userId = userId,
                    allCookies = allCookies
                };
                
                _logger.LogInformation("Auth info: {AuthInfo}", System.Text.Json.JsonSerializer.Serialize(authInfo));
                
                return Ok(new
                {
                    success = true,
                    isAuthenticated = !string.IsNullOrEmpty(token),
                    authInfo = authInfo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking auth status");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                _logger.LogInformation("=== GetCart called ===");
                
                // Get user ID from cookies or session
                var userId = GetUserIdFromCookies();
                if (userId == null)
                {
                    _logger.LogWarning("No user ID found in cookies");
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                _logger.LogInformation("User ID found: {UserId}", userId);
                
                var backendUrl = _configuration["BackendAPI:BaseUrl"];
                var client = _httpClientFactory.CreateClient("BackendAPI");
                
                var cartUrl = $"{backendUrl}/api/Cart?userId={userId}";
                _logger.LogInformation("Calling cart URL: {CartUrl}", cartUrl);
                
                // Forward cookies to backend
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, cartUrl);
                var cookieUserId = GetUserIdFromCookies();
                if (cookieUserId.HasValue)
                {
                    httpRequest.Headers.Add("Cookie", $"UserId={cookieUserId.Value}");
                }
                
                                 var response = await client.SendAsync(httpRequest);
                 var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Cart response status: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Cart response content length: {ContentLength}", content.Length);
                
                // Check if response is HTML
                if (content.TrimStart().StartsWith("<"))
                {
                    _logger.LogError("Backend returned HTML instead of JSON. First 200 chars: {Content}", content.Substring(0, Math.Min(200, content.Length)));
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Backend returned HTML instead of JSON",
                        content = content.Substring(0, Math.Min(500, content.Length))
                    });
                }
                
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var cartData = JsonSerializer.Deserialize<object>(content);
                        return Ok(new { success = true, data = cartData });
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, "Failed to parse JSON response");
                        return BadRequest(new 
                        { 
                            success = false, 
                            message = "Invalid JSON response",
                            content = content.Substring(0, Math.Min(500, content.Length))
                        });
                    }
                }
                else
                {
                    _logger.LogError("Backend returned error status: {StatusCode}, Content: {Content}", response.StatusCode, content);
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = $"Backend error: {response.StatusCode}",
                        content = content
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCart");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetCartSummary()
        {
            try
            {
                _logger.LogInformation("=== GetCartSummary called ===");
                
                var userId = GetUserIdFromCookies();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var backendUrl = _configuration["BackendAPI:BaseUrl"];
                var client = _httpClientFactory.CreateClient("BackendAPI");
                
                var summaryUrl = $"{backendUrl}/api/Cart/summary?userId={userId}";
                _logger.LogInformation("Calling summary URL: {SummaryUrl}", summaryUrl);
                
                var response = await client.GetAsync(summaryUrl);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Summary response status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    var summaryData = JsonSerializer.Deserialize<object>(content);
                    return Ok(new { success = true, data = summaryData });
                }
                else
                {
                    return BadRequest(new { success = false, message = $"Error: {response.StatusCode}", content = content });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCartSummary");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                _logger.LogInformation("=== AddToCart called ===");
                
                var userId = GetUserIdFromCookies();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                // Validate request
                if (request.Quantity <= 0)
                {
                    return BadRequest(new { success = false, message = "Số lượng phải lớn hơn 0" });
                }

                if (request.ProductId <= 0)
                {
                    return BadRequest(new { success = false, message = "ID sản phẩm không hợp lệ" });
                }

                var backendUrl = _configuration["BackendAPI:BaseUrl"];
                var client = _httpClientFactory.CreateClient("BackendAPI");
                
                // Request without userId (backend will get from cookies)
                var requestData = new
                {
                    productId = request.ProductId,
                    variantId = request.VariantId,
                    variantAttributes = request.VariantAttributes,
                    quantity = request.Quantity
                };
                
                var addUrl = $"{backendUrl}/api/Cart/add";
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Adding to cart for user {UserId}: {Json}", userId, json);
                
                // Forward cookies to backend
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, addUrl);
                
                // Forward all cookies from the current request
                var cookieHeader = Request.Headers["Cookie"].ToString();
                if (!string.IsNullOrEmpty(cookieHeader))
                {
                    httpRequest.Headers.Add("Cookie", cookieHeader);
                    _logger.LogInformation("Forwarding cookies: {CookieHeader}", cookieHeader);
                }
                else
                {
                    // Fallback: manually add UserId cookie
                    var cookieUserId = GetUserIdFromCookies();
                    if (cookieUserId.HasValue)
                    {
                        httpRequest.Headers.Add("Cookie", $"UserId={cookieUserId.Value}");
                        _logger.LogInformation("Added UserId cookie manually: {UserId}", cookieUserId.Value);
                    }
                }
                
                httpRequest.Content = content;
                
                var response = await client.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Add response status: {StatusCode}, Content: {Content}", response.StatusCode, responseContent);
                
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = JsonSerializer.Deserialize<object>(responseContent);
                        return Ok(new { success = true, message = "Thêm vào giỏ hàng thành công", data = result });
                    }
                    catch (JsonException)
                    {
                        return Ok(new { success = true, message = "Thêm vào giỏ hàng thành công" });
                    }
                }
                else
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<object>(responseContent);
                        return BadRequest(new { success = false, message = "Lỗi khi thêm vào giỏ hàng", data = errorResult });
                    }
                    catch (JsonException)
                    {
                        return BadRequest(new { success = false, message = "Lỗi khi thêm vào giỏ hàng", content = responseContent });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddToCart");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add-guest")]
        public async Task<IActionResult> AddToCartGuest([FromBody] AddToCartRequest request)
        {
            try
            {
                _logger.LogInformation("=== AddToCartGuest called ===");
                
                // Validate request
                if (request.Quantity <= 0)
                {
                    return BadRequest(new { success = false, message = "Số lượng phải lớn hơn 0" });
                }

                if (request.ProductId <= 0)
                {
                    return BadRequest(new { success = false, message = "ID sản phẩm không hợp lệ" });
                }

                var backendUrl = _configuration["BackendAPI:BaseUrl"];
                var client = _httpClientFactory.CreateClient("BackendAPI");
                
                // Request without userId (backend will get from cookies)
                var requestData = new
                {
                    productId = request.ProductId,
                    variantId = request.VariantId,
                    variantAttributes = request.VariantAttributes,
                    quantity = request.Quantity
                };
                
                var addUrl = $"{backendUrl}/api/Cart/add-guest";
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Adding to guest cart: {Json}", json);
                
                var response = await client.PostAsync(addUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Add guest response status: {StatusCode}, Content: {Content}", response.StatusCode, responseContent);
                
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = JsonSerializer.Deserialize<object>(responseContent);
                        return Ok(new { success = true, message = "Thêm vào giỏ hàng thành công", data = result });
                    }
                    catch (JsonException)
                    {
                        return Ok(new { success = true, message = "Thêm vào giỏ hàng thành công" });
                    }
                }
                else
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<object>(responseContent);
                        return BadRequest(new { success = false, message = "Lỗi khi thêm vào giỏ hàng", data = errorResult });
                    }
                    catch (JsonException)
                    {
                        return BadRequest(new { success = false, message = "Lỗi khi thêm vào giỏ hàng", content = responseContent });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddToCartGuest");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartRequest request)
        {
            try
            {
                _logger.LogInformation("=== UpdateCartItem called ===");
                
                var userId = GetUserIdFromCookies();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var backendUrl = _configuration["BackendAPI:BaseUrl"];
                var client = _httpClientFactory.CreateClient("BackendAPI");
                
                // Request without userId (backend will get from cookies)
                var requestData = new
                {
                    productId = request.ProductId,
                    variantId = request.VariantId,
                    variantAttributes = request.VariantAttributes,
                    quantity = request.Quantity
                };
                
                var updateUrl = $"{backendUrl}/api/Cart/update";
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Updating cart for user {UserId}: {Json}", userId, json);
                
                // Forward cookies to backend
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, updateUrl);
                var cookieUserId = GetUserIdFromCookies();
                if (cookieUserId.HasValue)
                {
                    httpRequest.Headers.Add("Cookie", $"UserId={cookieUserId.Value}");
                }
                httpRequest.Content = content;
                
                var response = await client.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Update response status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true, message = "Cập nhật giỏ hàng thành công" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Lỗi khi cập nhật giỏ hàng", content = responseContent });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCartItem");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveCartRequest request)
        {
            try
            {
                _logger.LogInformation("=== RemoveFromCart called ===");
                
                var userId = GetUserIdFromCookies();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var backendUrl = _configuration["BackendAPI:BaseUrl"];
                var client = _httpClientFactory.CreateClient("BackendAPI");
                
                // Request without userId (backend will get from cookies)
                var requestData = new
                {
                    productId = request.ProductId,
                    variantId = request.VariantId,
                    variantAttributes = request.VariantAttributes
                };
                
                var removeUrl = $"{backendUrl}/api/Cart/remove";
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Removing from cart for user {UserId}: {Json}", userId, json);
                
                // Forward cookies to backend
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, removeUrl);
                var cookieUserId = GetUserIdFromCookies();
                if (cookieUserId.HasValue)
                {
                    httpRequest.Headers.Add("Cookie", $"UserId={cookieUserId.Value}");
                }
                httpRequest.Content = content;
                
                var response = await client.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Remove response status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true, message = "Xóa sản phẩm khỏi giỏ hàng thành công" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Lỗi khi xóa sản phẩm", content = responseContent });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RemoveFromCart");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("increase-by-product")]
        public async Task<IActionResult> IncreaseCartItemByProduct([FromBody] IncreaseCartItemByProductRequest request)
        {
            try
            {
                _logger.LogInformation("=== IncreaseCartItemByProduct called ===");
                
                var userId = GetUserIdFromCookies();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var backendUrl = _configuration["BackendAPI:BaseUrl"];
                var client = _httpClientFactory.CreateClient("BackendAPI");
                
                // Request without userId (backend will get from cookies)
                var requestData = new
                {
                    productId = request.ProductId,
                    variantId = request.VariantId,
                    variantAttributes = request.VariantAttributes,
                    quantityToAdd = request.QuantityToAdd
                };
                
                var increaseUrl = $"{backendUrl}/api/Cart/increase-by-product";
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Increasing cart item for user {UserId}: {Json}", userId, json);
                
                // Forward cookies to backend
                var httpRequest = new HttpRequestMessage(HttpMethod.Put, increaseUrl);
                var cookieUserId = GetUserIdFromCookies();
                if (cookieUserId.HasValue)
                {
                    httpRequest.Headers.Add("Cookie", $"UserId={cookieUserId.Value}");
                }
                httpRequest.Content = content;
                
                var response = await client.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Increase response status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true, message = "Tăng số lượng sản phẩm thành công" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Lỗi khi tăng số lượng sản phẩm", content = responseContent });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in IncreaseCartItemByProduct");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("decrease-by-product")]
        public async Task<IActionResult> DecreaseCartItemByProduct([FromBody] DecreaseCartItemByProductRequest request)
        {
            try
            {
                _logger.LogInformation("=== DecreaseCartItemByProduct called ===");
                
                var userId = GetUserIdFromCookies();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var backendUrl = _configuration["BackendAPI:BaseUrl"];
                var client = _httpClientFactory.CreateClient("BackendAPI");
                
                // Request without userId (backend will get from cookies)
                var requestData = new
                {
                    productId = request.ProductId,
                    variantId = request.VariantId,
                    variantAttributes = request.VariantAttributes,
                    quantityToRemove = request.QuantityToRemove
                };
                
                var decreaseUrl = $"{backendUrl}/api/Cart/decrease-by-product";
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Decreasing cart item for user {UserId}: {Json}", userId, json);
                
                // Forward cookies to backend
                var httpRequest = new HttpRequestMessage(HttpMethod.Put, decreaseUrl);
                var cookieUserId = GetUserIdFromCookies();
                if (cookieUserId.HasValue)
                {
                    httpRequest.Headers.Add("Cookie", $"UserId={cookieUserId.Value}");
                }
                httpRequest.Content = content;
                
                var response = await client.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Decrease response status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true, message = "Giảm số lượng sản phẩm thành công" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Lỗi khi giảm số lượng sản phẩm", content = responseContent });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DecreaseCartItemByProduct");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Helper method to get user ID from cookies
        private int? GetUserIdFromCookies()
        {
            // Debug logging
            Console.WriteLine("=== GetUserIdFromCookies called ===");
            
            // Log all cookies
            var allCookies = Request.Cookies.Select(c => new { Key = c.Key, Value = c.Value }).ToList();
            Console.WriteLine($"Total cookies found: {allCookies.Count}");
            foreach (var cookie in allCookies)
            {
                Console.WriteLine($"Cookie: {cookie.Key} = {cookie.Value}");
            }
            
            // Try to get user ID from cookies
            var userIdStr = Request.Cookies["UserId"] ?? Request.Cookies["userId"];
            Console.WriteLine($"UserId from cookies: '{userIdStr}'");
            
            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                Console.WriteLine($"Successfully parsed userId: {userId}");
                return userId;
            }

            Console.WriteLine("No valid userId found in cookies");
            // If no user ID found, return null
            return null;
        }
    }

    // Request DTO for adding items to cart
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public string? VariantAttributes { get; set; }
        public int Quantity { get; set; }
    }

    // Request DTO for updating cart items
    public class UpdateCartRequest
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public string? VariantAttributes { get; set; }
        public int Quantity { get; set; }
    }

    // Request DTO for removing items from cart
    public class RemoveCartRequest
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public string? VariantAttributes { get; set; }
    }

    // Request DTO for increasing cart items by product
    public class IncreaseCartItemByProductRequest
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public string? VariantAttributes { get; set; }
        public int QuantityToAdd { get; set; }
    }

    // Request DTO for decreasing cart items by product
    public class DecreaseCartItemByProductRequest
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public string? VariantAttributes { get; set; }
        public int QuantityToRemove { get; set; }
    }
} 