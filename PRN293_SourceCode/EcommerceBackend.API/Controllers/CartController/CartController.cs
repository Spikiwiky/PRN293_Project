using EcommerceBackend.BusinessObject.Abstract;
using BusinessCartDto = EcommerceBackend.BusinessObject.dtos.CartDto;
using ApiCartDto = EcommerceBackend.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceBackend.API.Controllers.CartController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> GetUserCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var cart = await _cartService.GetUserCartAsync(userId);
                if (cart == null)
                {
                    return Ok(new ApiCartDto.CartResponseDto
                    {
                        Success = true,
                        Message = "Cart is empty",
                        Cart = null,
                        Summary = new ApiCartDto.CartSummaryDto { TotalItems = 0, TotalAmount = 0, CartItemCount = 0 }
                    });
                }

                var summary = await _cartService.GetCartSummaryAsync(userId);

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
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error retrieving cart: {ex.Message}"
                });
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> AddToCart([FromBody] ApiCartDto.AddToCartDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var businessRequest = new BusinessCartDto.AddToCartRequestDto
                {
                    ProductId = request.ProductId,
                    VariantId = request.VariantId,
                    VariantAttributes = request.VariantAttributes,
                    Quantity = request.Quantity
                };

                var result = await _cartService.AddToCartAsync(userId, businessRequest);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error adding item to cart: {ex.Message}"
                });
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> UpdateCartItem([FromBody] ApiCartDto.UpdateCartItemDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Validate that the cart item belongs to the current user
                if (!await _cartService.ValidateCartItemAsync(request.CartDetailId, userId))
                {
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

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
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
                var userId = GetCurrentUserId();
                
                if (!await _cartService.ValidateCartItemAsync(request.CartDetailId, userId))
                {
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

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error increasing cart item quantity: {ex.Message}"
                });
            }
        }

        [HttpPut("decrease")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> DecreaseCartItem([FromBody] ApiCartDto.DecreaseCartItemDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                if (!await _cartService.ValidateCartItemAsync(request.CartDetailId, userId))
                {
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

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error decreasing cart item quantity: {ex.Message}"
                });
            }
        }

        [HttpDelete("remove")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> RemoveFromCart([FromBody] ApiCartDto.RemoveFromCartDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                if (!await _cartService.ValidateCartItemAsync(request.CartDetailId, userId))
                {
                    return BadRequest(new ApiCartDto.CartResponseDto
                    {
                        Success = false,
                        Message = "Cart item not found or does not belong to current user"
                    });
                }

                var result = await _cartService.RemoveFromCartAsync(request.CartDetailId);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error removing item from cart: {ex.Message}"
                });
            }
        }

        [HttpDelete("clear")]
        public async Task<ActionResult<ApiCartDto.CartResponseDto>> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var result = await _cartService.ClearCartAsync(userId);

                return Ok(MapToApiResponseDto(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiCartDto.CartResponseDto
                {
                    Success = false,
                    Message = $"Error clearing cart: {ex.Message}"
                });
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ApiCartDto.CartSummaryDto>> GetCartSummary()
        {
            try
            {
                var userId = GetCurrentUserId();
                var summary = await _cartService.GetCartSummaryAsync(userId);

                return Ok(MapToApiSummaryDto(summary));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error getting cart summary: {ex.Message}" });
            }
        }

        // Helper methods
        private int GetCurrentUserId()
        {
            // This should be implemented based on your JWT authentication system
            // For now, returning a placeholder
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            
            throw new UnauthorizedAccessException("User ID not found in token");
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