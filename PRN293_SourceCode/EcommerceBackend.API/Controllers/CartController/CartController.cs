using EcommerceBackend.BusinessObject.Abstract;
using BusinessCartDto = EcommerceBackend.BusinessObject.dtos.CartDto;
using ApiCartDto = EcommerceBackend.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Text.Json;


namespace EcommerceBackend.API.Controllers.CartController
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            _logger.LogInformation("=== Test endpoint called ===");
            return Ok(new { message = "CartController is working", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> GetUserCart()
        {
            try
            {
                _logger.LogInformation("=== GetUserCart method called ===");
                
                // Get userId from cookies
                var userIdStr = Request.Cookies["UserId"];
                if (string.IsNullOrEmpty(userIdStr))
                {
                    _logger.LogWarning("No UserId found in cookies");
                    return Unauthorized(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Vui lòng đăng nhập"
                    });
                }

                if (!int.TryParse(userIdStr, out int userId))
                {
                    _logger.LogWarning("Invalid UserId in cookies: {UserIdStr}", userIdStr);
                    return Unauthorized(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Thông tin người dùng không hợp lệ"
                    });
                }

                _logger.LogInformation("Getting cart for user ID: {UserId}", userId);
                
                // Debug: Check if user exists
                _logger.LogInformation("Checking if user {UserId} exists in database...", userId);
                
                var cart = await _cartService.GetUserCartAsync(userId);
                _logger.LogInformation("Cart result: {Cart}", cart != null ? "Found" : "Not found");
                
                if (cart == null)
                {
                    _logger.LogInformation("Cart is empty for user ID: {UserId}", userId);
                    return Ok(new ApiCartDto.CartResponseDto
                    {
                        Success = true,
                        Message = "Cart is empty",
                        Cart = null,
                        Summary = new ApiCartDto.CartSummaryDto { TotalItems = 0, TotalAmount = 0, CartItemCount = 0 }
                    });
                }

                var summary = await _cartService.GetCartSummaryAsync(cart.CustomerId);
                _logger.LogInformation("Successfully retrieved cart for user ID: {UserId}", userId);

                return Ok(new ApiCartDto.CartResponseDto
                {
                    Success = true,
                    Message = "Cart retrieved successfully",
                    Cart = MapToApiCartDto(cart),
                    Summary = MapToApiSummaryDto(summary)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart");
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error retrieving cart: {ex.Message}"
                });
            }
        }

        [HttpGet("items")]
        public async Task<ActionResult<object>> GetCartItems()
        {
            try
            {
                _logger.LogInformation("=== GetCartItems method called ===");
                
                // Get userId from cookies
                var userIdStr = Request.Cookies["UserId"];
                if (string.IsNullOrEmpty(userIdStr))
                {
                    _logger.LogWarning("No UserId found in cookies");
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                if (!int.TryParse(userIdStr, out int userId))
                {
                    _logger.LogWarning("Invalid UserId in cookies: {UserIdStr}", userIdStr);
                    return Unauthorized(new { success = false, message = "Thông tin người dùng không hợp lệ" });
                }

                _logger.LogInformation("Getting cart items for user ID: {UserId}", userId);
                
                var cartItems = await _cartService.GetCartItemsByUserIdAsync(userId);
                _logger.LogInformation("Cart items retrieved: {Count} items", cartItems.Count);

                return Ok(new { success = true, data = cartItems });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart items");
                return BadRequest(new { success = false, message = $"Error retrieving cart items: {ex.Message}" });
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> AddToCart([FromBody] ApiCartDto.AddToCartDto request)
        {
            try
            {
                // Get userId from cookies
                var userIdStr = Request.Cookies["UserId"];
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    _logger.LogWarning("Invalid or missing UserId in cookies: {UserIdStr}", userIdStr);
                    return Unauthorized(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Vui lòng đăng nhập"
                    });
                }

                _logger.LogInformation("Adding item to cart for user ID: {UserId}, ProductId: {ProductId}, VariantId: {VariantId}, Quantity: {Quantity}", 
                    userId, request.ProductId, request.VariantId, request.Quantity);
                
                // Validate request
                if (request.Quantity <= 0)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Số lượng phải lớn hơn 0"
                    });
                }

                if (request.ProductId <= 0)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "ID sản phẩm không hợp lệ"
                    });
                }
                
                var businessRequest = new BusinessCartDto.AddToCartRequestDto
                {
                    ProductId = request.ProductId,
                    VariantId = request.VariantId,
                    VariantAttributes = request.VariantAttributes,
                    Quantity = request.Quantity
                };

                var result = await _cartService.AddToCartAsync(userId, businessRequest);
                _logger.LogInformation("Successfully added item to cart for user ID: {UserId}", userId);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error adding item to cart: {ex.Message}"
                });
            }
        }

        [HttpPost("add-guest")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> AddToCartGuest([FromBody] ApiCartDto.AddToCartDto request)
        {
            try
            {
                _logger.LogInformation("Adding item to guest cart, ProductId: {ProductId}, VariantId: {VariantId}, Quantity: {Quantity}", 
                    request.ProductId, request.VariantId, request.Quantity);
                
                // Validate request
                if (request.Quantity <= 0)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Số lượng phải lớn hơn 0"
                    });
                }

                if (request.ProductId <= 0)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "ID sản phẩm không hợp lệ"
                    });
                }
                
                // For guest users, we'll use a session-based approach
                // Generate a unique guest session ID or use a temporary approach
                var guestSessionId = Guid.NewGuid().ToString();
                
                // For now, we'll use a temporary approach by creating a cart with null CustomerId
                // This requires modifying the database schema or using a different approach
                
                // Alternative: Create a temporary guest user in the database
                // For now, let's use a simple approach with a known guest user ID
                var guestUserId = 999; // This should be created in the database first
                
                // Check if guest user cart exists, if not create one
                try
                {
                    var guestUser = await _cartService.GetUserCartAsync(guestUserId);
                    if (guestUser == null)
                    {
                        _logger.LogInformation("Guest user cart not found, creating new cart for guest user ID: {GuestUserId}", guestUserId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Guest user validation failed: {Error}. Proceeding with cart creation.", ex.Message);
                }
                
                var businessRequest = new BusinessCartDto.AddToCartRequestDto
                {
                    ProductId = request.ProductId,
                    VariantId = request.VariantId,
                    VariantAttributes = request.VariantAttributes,
                    Quantity = request.Quantity
                };

                var result = await _cartService.AddToCartAsync(guestUserId, businessRequest);
                _logger.LogInformation("Successfully added item to guest cart");

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to guest cart");
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error adding item to cart: {ex.Message}"
                });
            }
        }

        [HttpPut("update-by-id")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> UpdateCartItem([FromBody] ApiCartDto.UpdateCartItemDto request)
        {
            try
            {
                var userId = request.UserId; // Lấy từ request body
                _logger.LogInformation("Updating cart item for user ID: {UserId}, CartDetailId: {CartDetailId}", userId, request.CartDetailId);
                
                var cart = await _cartService.GetUserCartAsync(userId);
                if (cart == null)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart not found"
                    });
                }
                
                // Validate that the cart item belongs to the current user
                if (!await _cartService.ValidateCartItemAsync(request.CartDetailId, cart.CustomerId))
                {
                    _logger.LogWarning("Cart item validation failed for user ID: {UserId}, CartDetailId: {CartDetailId}", userId, request.CartDetailId);
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart item not found or does not belong to current user"
                    });
                }

                var businessRequest = new BusinessCartDto.UpdateCartItemRequestDto
                {
                    CartDetailId = request.CartDetailId,
                    Quantity = request.Quantity
                };

                var result = await _cartService.UpdateCartItemQuantityAsync(request.CartDetailId, businessRequest);
                _logger.LogInformation("Successfully updated cart item for user ID: {UserId}", userId);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item");
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error updating cart item: {ex.Message}"
                });
            }
        }

        [HttpPost("update")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> UpdateCartItemByProduct([FromBody] ApiCartDto.UpdateCartItemByProductDto request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogError("Request is null");
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Invalid request format"
                    });
                }

                // Get userId from cookies
                var userIdStr = Request.Cookies["UserId"];
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    _logger.LogWarning("Invalid or missing UserId in cookies: {UserIdStr}", userIdStr);
                    return Unauthorized(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Vui lòng đăng nhập"
                    });
                }

                _logger.LogInformation("Updating cart item by product for user ID: {UserId}, ProductId: {ProductId}, VariantId: {VariantId}, Quantity: {Quantity}", 
                    userId, request.ProductId, request.VariantId, request.Quantity);
                _logger.LogInformation("Request VariantId: {VariantIdValue}", 
                    request.VariantId?.ToString() ?? "null");
                
                // Debug: Log the entire request object
                _logger.LogInformation("Full request object: {@Request}", request);
                
                // Debug: Log ModelState errors if any
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogError("ModelState errors: {Errors}", string.Join(", ", errors));
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = $"Validation errors: {string.Join(", ", errors)}"
                    });
                }
                
                var cart = await _cartService.GetUserCartAsync(userId);
                if (cart == null)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart not found"
                    });
                }

                // Find cart item by product and variant
                _logger.LogInformation("Looking for cart item with ProductId: {ProductId}, VariantId: {VariantId}, VariantAttributes: {VariantAttributes}", 
                    request.ProductId, request.VariantId, request.VariantAttributes);
                
                var cartItem = cart.CartDetails?.FirstOrDefault(cd => 
                {
                    var productMatch = cd.ProductId == request.ProductId;
                    var variantMatch = cd.VariantId == request.VariantId;
                    
                    // Kiểm tra variantAttributes nếu có
                    var attributesMatch = true;
                    if (!string.IsNullOrEmpty(request.VariantAttributes))
                    {
                        attributesMatch = AreVariantAttributesEqual(cd.VariantAttributes, request.VariantAttributes);
                    }
                    
                    _logger.LogInformation("Checking cart item - ProductId: {CartProductId}, VariantId: {CartVariantId}, VariantAttributes: {CartVariantAttributes}, ProductMatch: {ProductMatch}, VariantMatch: {VariantMatch}, AttributesMatch: {AttributesMatch}", 
                        cd.ProductId, cd.VariantId, cd.VariantAttributes, productMatch, variantMatch, attributesMatch);
                    
                    return productMatch && variantMatch && attributesMatch;
                });

                if (cartItem == null)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart item not found"
                    });
                }

                var businessRequest = new BusinessCartDto.UpdateCartItemRequestDto
                {
                    CartDetailId = cartItem.CartDetailId,
                    Quantity = request.Quantity
                };

                var result = await _cartService.UpdateCartItemQuantityAsync(cartItem.CartDetailId, businessRequest);
                _logger.LogInformation("Successfully updated cart item for user ID: {UserId}", userId);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item by product");
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error updating cart item: {ex.Message}"
                });
            }
        }

        [HttpPut("increase")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> IncreaseCartItem([FromBody] ApiCartDto.IncreaseCartItemDto request)
        {
            try
            {
                var userId = request.UserId; // Lấy từ request body
                _logger.LogInformation("Increasing cart item for user ID: {UserId}, CartDetailId: {CartDetailId}", userId, request.CartDetailId);
                
                var cart = await _cartService.GetUserCartAsync(userId);
                if (cart == null)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart not found"
                    });
                }
                
                if (!await _cartService.ValidateCartItemAsync(request.CartDetailId, cart.CustomerId))
                {
                    _logger.LogWarning("Cart item validation failed for user ID: {UserId}, CartDetailId: {CartDetailId}", userId, request.CartDetailId);
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart item not found or does not belong to current user"
                    });
                }

                var businessRequest = new BusinessCartDto.IncreaseCartItemRequestDto
                {
                    CartDetailId = request.CartDetailId,
                    QuantityToAdd = request.QuantityToAdd
                };

                var result = await _cartService.IncreaseCartItemQuantityAsync(request.CartDetailId, businessRequest);
                _logger.LogInformation("Successfully increased cart item for user ID: {UserId}", userId);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error increasing cart item quantity");
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error increasing cart item quantity: {ex.Message}"
                });
            }
        }

        [HttpPut("increase-by-product")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> IncreaseCartItemByProduct([FromBody] ApiCartDto.IncreaseCartItemByProductDto request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogError("Request is null");
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Invalid request format"
                    });
                }

                // Get userId from cookies
                var userIdStr = Request.Cookies["UserId"];
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    _logger.LogWarning("Invalid or missing UserId in cookies: {UserIdStr}", userIdStr);
                    return Unauthorized(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Vui lòng đăng nhập"
                    });
                }

                _logger.LogInformation("Increasing cart item by product for user ID: {UserId}, ProductId: {ProductId}, VariantId: {VariantId}, QuantityToAdd: {QuantityToAdd}", 
                    userId, request.ProductId, request.VariantId, request.QuantityToAdd);
                _logger.LogInformation("Request VariantId: {VariantIdValue}", 
                    request.VariantId?.ToString() ?? "null");
                
                // Debug: Log the entire request object
                _logger.LogInformation("Full request object: {@Request}", request);
                
                // Debug: Log ModelState errors if any
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogError("ModelState errors: {Errors}", string.Join(", ", errors));
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = $"Validation errors: {string.Join(", ", errors)}"
                    });
                }
                
                var cart = await _cartService.GetUserCartAsync(userId);
                if (cart == null)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart not found"
                    });
                }

                // Find cart item by product and variant
                _logger.LogInformation("Looking for cart item with ProductId: {ProductId}, VariantId: {VariantId}, VariantAttributes: {VariantAttributes}", 
                    request.ProductId, request.VariantId, request.VariantAttributes);
                
                var cartItem = cart.CartDetails?.FirstOrDefault(cd => 
                {
                    var productMatch = cd.ProductId == request.ProductId;
                    var variantMatch = cd.VariantId == request.VariantId;
                    
                    // Kiểm tra variantAttributes nếu có
                    var attributesMatch = true;
                    if (!string.IsNullOrEmpty(request.VariantAttributes))
                    {
                        attributesMatch = AreVariantAttributesEqual(cd.VariantAttributes, request.VariantAttributes);
                    }
                    
                    _logger.LogInformation("Checking cart item - ProductId: {CartProductId}, VariantId: {CartVariantId}, VariantAttributes: {CartVariantAttributes}, ProductMatch: {ProductMatch}, VariantMatch: {VariantMatch}, AttributesMatch: {AttributesMatch}", 
                        cd.ProductId, cd.VariantId, cd.VariantAttributes, productMatch, variantMatch, attributesMatch);
                    
                    return productMatch && variantMatch && attributesMatch;
                });

                if (cartItem == null)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart item not found"
                    });
                }

                var businessRequest = new BusinessCartDto.IncreaseCartItemRequestDto
                {
                    CartDetailId = cartItem.CartDetailId,
                    QuantityToAdd = request.QuantityToAdd
                };

                var result = await _cartService.IncreaseCartItemQuantityAsync(cartItem.CartDetailId, businessRequest);
                _logger.LogInformation("Successfully increased cart item for user ID: {UserId}", userId);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error increasing cart item by product");
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error increasing cart item: {ex.Message}"
                });
            }
        }

        [HttpPut("decrease")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> DecreaseCartItem([FromBody] ApiCartDto.DecreaseCartItemDto request)
        {
            try
            {
                var userId = request.UserId; // Lấy từ request body
                _logger.LogInformation("Decreasing cart item for user ID: {UserId}, CartDetailId: {CartDetailId}", userId, request.CartDetailId);
                
                var cart = await _cartService.GetUserCartAsync(userId);
                if (cart == null)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart not found"
                    });
                }
                
                if (!await _cartService.ValidateCartItemAsync(request.CartDetailId, cart.CustomerId))
                {
                    _logger.LogWarning("Cart item validation failed for user ID: {UserId}, CartDetailId: {CartDetailId}", userId, request.CartDetailId);
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart item not found or does not belong to current user"
                    });
                }

                var businessRequest = new BusinessCartDto.DecreaseCartItemRequestDto
                {
                    CartDetailId = request.CartDetailId,
                    QuantityToRemove = request.QuantityToRemove
                };

                var result = await _cartService.DecreaseCartItemQuantityAsync(request.CartDetailId, businessRequest);
                _logger.LogInformation("Successfully decreased cart item for user ID: {UserId}", userId);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decreasing cart item quantity");
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error decreasing cart item quantity: {ex.Message}"
                });
            }
        }

        [HttpPut("decrease-by-product")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> DecreaseCartItemByProduct([FromBody] ApiCartDto.DecreaseCartItemByProductDto request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogError("Request is null");
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Invalid request format"
                    });
                }

                // Get userId from cookies
                var userIdStr = Request.Cookies["UserId"];
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    _logger.LogWarning("Invalid or missing UserId in cookies: {UserIdStr}", userIdStr);
                    return Unauthorized(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Vui lòng đăng nhập"
                    });
                }

                _logger.LogInformation("Decreasing cart item by product for user ID: {UserId}, ProductId: {ProductId}, VariantId: {VariantId}, QuantityToRemove: {QuantityToRemove}", 
                    userId, request.ProductId, request.VariantId, request.QuantityToRemove);
                _logger.LogInformation("Request VariantId: {VariantIdValue}", 
                    request.VariantId?.ToString() ?? "null");
                
                // Debug: Log the entire request object
                _logger.LogInformation("Full request object: {@Request}", request);
                
                // Debug: Log ModelState errors if any
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogError("ModelState errors: {Errors}", string.Join(", ", errors));
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = $"Validation errors: {string.Join(", ", errors)}"
                    });
                }
                
                var cart = await _cartService.GetUserCartAsync(userId);
                if (cart == null)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart not found"
                    });
                }

                // Find cart item by product and variant
                _logger.LogInformation("Looking for cart item with ProductId: {ProductId}, VariantId: {VariantId}, VariantAttributes: {VariantAttributes}", 
                    request.ProductId, request.VariantId, request.VariantAttributes);
                
                var cartItem = cart.CartDetails?.FirstOrDefault(cd => 
                {
                    var productMatch = cd.ProductId == request.ProductId;
                    var variantMatch = cd.VariantId == request.VariantId;
                    
                    // Kiểm tra variantAttributes nếu có
                    var attributesMatch = true;
                    if (!string.IsNullOrEmpty(request.VariantAttributes))
                    {
                        attributesMatch = AreVariantAttributesEqual(cd.VariantAttributes, request.VariantAttributes);
                    }
                    
                    _logger.LogInformation("Checking cart item - ProductId: {CartProductId}, VariantId: {CartVariantId}, VariantAttributes: {CartVariantAttributes}, ProductMatch: {ProductMatch}, VariantMatch: {VariantMatch}, AttributesMatch: {AttributesMatch}", 
                        cd.ProductId, cd.VariantId, cd.VariantAttributes, productMatch, variantMatch, attributesMatch);
                    
                    return productMatch && variantMatch && attributesMatch;
                });

                if (cartItem == null)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart item not found"
                    });
                }

                var businessRequest = new BusinessCartDto.DecreaseCartItemRequestDto
                {
                    CartDetailId = cartItem.CartDetailId,
                    QuantityToRemove = request.QuantityToRemove
                };

                var result = await _cartService.DecreaseCartItemQuantityAsync(cartItem.CartDetailId, businessRequest);
                _logger.LogInformation("Successfully decreased cart item for user ID: {UserId}", userId);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decreasing cart item by product");
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error decreasing cart item: {ex.Message}"
                });
            }
        }

        [HttpDelete("remove")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> RemoveFromCart([FromBody] ApiCartDto.RemoveFromCartDto request)
        {
            try
            {
                var userId = request.UserId; // Lấy từ request body
                _logger.LogInformation("Removing item from cart for user ID: {UserId}, CartDetailId: {CartDetailId}", userId, request.CartDetailId);
                
                var cart = await _cartService.GetUserCartAsync(userId);
                if (cart == null)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart not found"
                    });
                }
                
                if (!await _cartService.ValidateCartItemAsync(request.CartDetailId, cart.CustomerId))
                {
                    _logger.LogWarning("Cart item validation failed for user ID: {UserId}, CartDetailId: {CartDetailId}", userId, request.CartDetailId);
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart item not found or does not belong to current user"
                    });
                }

                var result = await _cartService.RemoveFromCartAsync(request.CartDetailId);
                _logger.LogInformation("Successfully removed item from cart for user ID: {UserId}", userId);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart");
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error removing item from cart: {ex.Message}"
                });
            }
        }

        [HttpPost("remove")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> RemoveFromCartByProduct([FromBody] ApiCartDto.RemoveFromCartByProductDto request)
        {
            try
            {
                // Get userId from cookies
                var userIdStr = Request.Cookies["UserId"];
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    _logger.LogWarning("Invalid or missing UserId in cookies: {UserIdStr}", userIdStr);
                    return Unauthorized(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Vui lòng đăng nhập"
                    });
                }

                _logger.LogInformation("Removing item from cart by product for user ID: {UserId}, ProductId: {ProductId}, VariantId: {VariantId}", 
                    userId, request.ProductId, request.VariantId);
                
                var cart = await _cartService.GetUserCartAsync(userId);
                if (cart == null)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart not found"
                    });
                }

                // Find cart item by product and variant
                _logger.LogInformation("Looking for cart item to remove with ProductId: {ProductId}, VariantId: {VariantId}, VariantAttributes: {VariantAttributes}", 
                    request.ProductId, request.VariantId, request.VariantAttributes);
                
                var cartItem = cart.CartDetails?.FirstOrDefault(cd => 
                {
                    var productMatch = cd.ProductId == request.ProductId;
                    var variantMatch = cd.VariantId == request.VariantId;
                    
                    // Kiểm tra variantAttributes nếu có
                    var attributesMatch = true;
                    if (!string.IsNullOrEmpty(request.VariantAttributes))
                    {
                        attributesMatch = AreVariantAttributesEqual(cd.VariantAttributes, request.VariantAttributes);
                    }
                    
                    _logger.LogInformation("Checking cart item for removal - ProductId: {CartProductId}, VariantId: {CartVariantId}, VariantAttributes: {CartVariantAttributes}, ProductMatch: {ProductMatch}, VariantMatch: {VariantMatch}, AttributesMatch: {AttributesMatch}", 
                        cd.ProductId, cd.VariantId, cd.VariantAttributes, productMatch, variantMatch, attributesMatch);
                    
                    return productMatch && variantMatch && attributesMatch;
                });

                if (cartItem == null)
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart item not found"
                    });
                }

                var result = await _cartService.RemoveFromCartAsync(cartItem.CartDetailId);
                _logger.LogInformation("Successfully removed item from cart for user ID: {UserId}", userId);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart by product");
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error removing item from cart: {ex.Message}"
                });
            }
        }

        [HttpDelete("clear")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> ClearCart([FromQuery] int userId)
        {
            try
            {
                _logger.LogInformation("Clearing cart for user ID: {UserId}", userId);
                
                var cart = await _cartService.GetUserCartAsync(userId);
                if (cart == null)
                {
                    return Ok(new ApiCartDto.CartResponseDto
                    {
                        Success = true,
                        Message = "Cart is already empty",
                        Cart = null,
                        Summary = new ApiCartDto.CartSummaryDto { TotalItems = 0, TotalAmount = 0, CartItemCount = 0 }
                    });
                }
                
                var result = await _cartService.ClearCartAsync(cart.CustomerId);
                _logger.LogInformation("Successfully cleared cart for user ID: {UserId}", userId);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error clearing cart: {ex.Message}"
                });
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ApiCartDto.CartSummaryDto>> GetCartSummary([FromQuery] int userId)
        {
            try
            {
                _logger.LogInformation("Getting cart summary for user ID: {UserId}", userId);
                
                var cart = await _cartService.GetUserCartAsync(userId);
                if (cart == null)
                {
                    return Ok(new ApiCartDto.CartSummaryDto { TotalItems = 0, TotalAmount = 0, CartItemCount = 0 });
                }
                
                var summary = await _cartService.GetCartSummaryAsync(cart.CustomerId);
                _logger.LogInformation("Successfully retrieved cart summary for user ID: {UserId}", userId);

                return Ok(MapToApiSummaryDto(summary));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart summary");
                return BadRequest(new { Message = $"Error getting cart summary: {ex.Message}" });
            }
        }

        // Helper: So sánh JSON cho variantAttributes
        private bool AreVariantAttributesEqual(string? a, string? b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)) return true;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            
            try
            {
                // Parse JSON objects
                var objA = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(a);
                var objB = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(b);
                
                if (objA == null && objB == null) return true;
                if (objA == null || objB == null) return false;
                
                // Compare sorted key-value pairs
                var sortedA = objA.OrderBy(x => x.Key).ToList();
                var sortedB = objB.OrderBy(x => x.Key).ToList();
                
                if (sortedA.Count != sortedB.Count) return false;
                
                for (int i = 0; i < sortedA.Count; i++)
                {
                    if (sortedA[i].Key != sortedB[i].Key || 
                        !string.Equals(sortedA[i].Value?.ToString(), sortedB[i].Value?.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                // Fallback to string comparison if JSON parsing fails
                _logger.LogWarning("JSON parsing failed for variant attributes comparison: {Error}. Falling back to string comparison.", ex.Message);
                return string.Equals(a?.Trim(), b?.Trim(), StringComparison.OrdinalIgnoreCase);
            }
        }

        // Mapping methods
        private ApiCartDto.CartResponseDto MapToApiResponseDto(BusinessCartDto.CartOperationResultDto businessResult)
        {
            return new ApiCartDto.CartResponseDto
            {
                Success = businessResult.Success,
                Message = businessResult.Message,
                Cart = businessResult.Cart != null ? MapToApiCartDto(businessResult.Cart) : null,
                Summary = businessResult.Summary != null ? MapToApiSummaryDto(businessResult.Summary) : null
            };
        }

        private ApiCartDto.CartDto MapToApiCartDto(BusinessCartDto.CartResponseDto businessCart)
        {
            return new ApiCartDto.CartDto
            {
                CartId = businessCart.CartId,
                CustomerId = businessCart.CustomerId,
                TotalQuantity = businessCart.TotalQuantity,
                AmountDue = businessCart.AmountDue,
                CreatedAt = businessCart.CreatedAt,
                UpdatedAt = businessCart.UpdatedAt,
                CartDetails = businessCart.CartDetails.Select(cd => new ApiCartDto.CartDetailDto
                {
                    CartDetailId = cd.CartDetailId,
                    ProductId = cd.ProductId,
                    VariantId = cd.VariantId,
                    ProductName = cd.ProductName,
                    Quantity = cd.Quantity,
                    Price = cd.Price,
                    VariantAttributes = cd.VariantAttributes,
                    ProductImage = cd.ProductImage,
                    ProductDescription = cd.ProductDescription
                }).ToList()
            };
        }

        private ApiCartDto.CartSummaryDto MapToApiSummaryDto(BusinessCartDto.CartSummaryResponseDto businessSummary)
        {
            return new ApiCartDto.CartSummaryDto
            {
                TotalItems = businessSummary.TotalItems,
                TotalAmount = businessSummary.TotalAmount,
                CartItemCount = businessSummary.CartItemCount
            };
        }
    }
} 
