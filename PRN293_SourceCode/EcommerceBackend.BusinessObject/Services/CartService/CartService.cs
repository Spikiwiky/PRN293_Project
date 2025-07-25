using EcommerceBackend.BusinessObject.Abstract;
using EcommerceBackend.BusinessObject.dtos.CartDto;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.DataAccess.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EcommerceBackend.BusinessObject.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<CartResponseDto?> GetUserCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(userId);
            if (cart == null)
                return null;

            return MapToCartResponseDto(cart);
        }

        public async Task<CartResponseDto> CreateUserCartAsync(int userId)
        {
            // Check if user already has a cart
            var existingCart = await _cartRepository.GetCartByCustomerIdAsync(userId);
            if (existingCart != null)
                return MapToCartResponseDto(existingCart);

            var cart = await _cartRepository.CreateCartAsync(userId);
            return MapToCartResponseDto(cart);
        }

        public async Task<CartOperationResultDto> AddToCartAsync(int userId, AddToCartRequestDto request)
        {
            try
            {
                // Validate product and quantity
                if (!await ValidateProductForCartAsync(request.ProductId, request.Quantity))
                    return new CartOperationResultDto
                    {
                        Success = false,
                        Message = "Invalid product or quantity"
                    };

                // Get or create user cart
                var cart = await GetUserCartAsync(userId) ?? await CreateUserCartAsync(userId);

                // Add item to cart
                var cartItem = await _cartRepository.AddItemToCartAsync(
                    cart.CartId, 
                    request.ProductId, 
                    request.VariantId, 
                    request.VariantAttributes, 
                    request.Quantity
                );

                var updatedCart = await GetUserCartAsync(userId);
                var summary = await GetCartSummaryAsync(userId);

                return new CartOperationResultDto
                {
                    Success = true,
                    Message = "Item added to cart successfully",
                    Cart = updatedCart,
                    Summary = summary
                };
            }
            catch (Exception ex)
            {
                return new CartOperationResultDto
                {
                    Success = false,
                    Message = $"Error adding item to cart: {ex.Message}"
                };
            }
        }

        public async Task<CartDetailResponseDto?> GetCartItemAsync(int cartDetailId)
        {
            var cartItem = await _cartRepository.GetCartItemAsync(cartDetailId);
            if (cartItem == null)
                return null;

            return MapToCartDetailResponseDto(cartItem);
        }

        public async Task<CartOperationResultDto> UpdateCartItemQuantityAsync(int cartDetailId, UpdateCartItemRequestDto request)
        {
            try
            {
                if (request.Quantity <= 0)
                    return new CartOperationResultDto
                    {
                        Success = false,
                        Message = "Quantity must be greater than 0"
                    };

                var cartItem = await _cartRepository.UpdateCartItemAsync(cartDetailId, request.Quantity);
                if (cartItem == null)
                    return new CartOperationResultDto
                    {
                        Success = false,
                        Message = "Cart item not found"
                    };

                var cart = await GetUserCartAsync(cartItem.Cart.CustomerId ?? 0);
                var summary = await GetCartSummaryAsync(cartItem.Cart.CustomerId ?? 0);

                return new CartOperationResultDto
                {
                    Success = true,
                    Message = "Cart item updated successfully",
                    Cart = cart,
                    Summary = summary
                };
            }
            catch (Exception ex)
            {
                return new CartOperationResultDto
                {
                    Success = false,
                    Message = $"Error updating cart item: {ex.Message}"
                };
            }
        }

        public async Task<CartOperationResultDto> IncreaseCartItemQuantityAsync(int cartDetailId, IncreaseCartItemRequestDto request)
        {
            try
            {
                if (request.QuantityToAdd <= 0)
                    return new CartOperationResultDto
                    {
                        Success = false,
                        Message = "Quantity to add must be greater than 0"
                    };

                var cartItem = await _cartRepository.IncreaseCartItemQuantityAsync(cartDetailId, request.QuantityToAdd);
                if (cartItem == null)
                    return new CartOperationResultDto
                    {
                        Success = false,
                        Message = "Cart item not found"
                    };

                var cart = await GetUserCartAsync(cartItem.Cart.CustomerId ?? 0);
                var summary = await GetCartSummaryAsync(cartItem.Cart.CustomerId ?? 0);

                return new CartOperationResultDto
                {
                    Success = true,
                    Message = "Cart item quantity increased successfully",
                    Cart = cart,
                    Summary = summary
                };
            }
            catch (Exception ex)
            {
                return new CartOperationResultDto
                {
                    Success = false,
                    Message = $"Error increasing cart item quantity: {ex.Message}"
                };
            }
        }

        public async Task<CartOperationResultDto> DecreaseCartItemQuantityAsync(int cartDetailId, DecreaseCartItemRequestDto request)
        {
            try
            {
                if (request.QuantityToRemove <= 0)
                    return new CartOperationResultDto
                    {
                        Success = false,
                        Message = "Quantity to remove must be greater than 0"
                    };

                var cartItem = await _cartRepository.DecreaseCartItemQuantityAsync(cartDetailId, request.QuantityToRemove);
                var cart = await GetUserCartAsync(cartItem?.Cart.CustomerId ?? 0);
                var summary = await GetCartSummaryAsync(cartItem?.Cart.CustomerId ?? 0);

                return new CartOperationResultDto
                {
                    Success = true,
                    Message = cartItem == null ? "Cart item removed (quantity reached 0)" : "Cart item quantity decreased successfully",
                    Cart = cart,
                    Summary = summary
                };
            }
            catch (Exception ex)
            {
                return new CartOperationResultDto
                {
                    Success = false,
                    Message = $"Error decreasing cart item quantity: {ex.Message}"
                };
            }
        }

        public async Task<CartOperationResultDto> RemoveFromCartAsync(int cartDetailId)
        {
            try
            {
                var cartItem = await _cartRepository.GetCartItemAsync(cartDetailId);
                if (cartItem == null)
                    return new CartOperationResultDto
                    {
                        Success = false,
                        Message = "Cart item not found"
                    };

                var userId = cartItem.Cart.CustomerId ?? 0;
                var success = await _cartRepository.RemoveCartItemAsync(cartDetailId);
                
                if (!success)
                    return new CartOperationResultDto
                    {
                        Success = false,
                        Message = "Failed to remove cart item"
                    };

                var cart = await GetUserCartAsync(userId);
                var summary = await GetCartSummaryAsync(userId);

                return new CartOperationResultDto
                {
                    Success = true,
                    Message = "Item removed from cart successfully",
                    Cart = cart,
                    Summary = summary
                };
            }
            catch (Exception ex)
            {
                return new CartOperationResultDto
                {
                    Success = false,
                    Message = $"Error removing item from cart: {ex.Message}"
                };
            }
        }

        public async Task<CartOperationResultDto> ClearCartAsync(int userId)
        {
            try
            {
                var cart = await GetUserCartAsync(userId);
                if (cart == null)
                    return new CartOperationResultDto
                    {
                        Success = false,
                        Message = "Cart is already empty"
                    };

                var success = await _cartRepository.ClearCartAsync(cart.CartId);
                if (!success)
                    return new CartOperationResultDto
                    {
                        Success = false,
                        Message = "Failed to clear cart"
                    };

                return new CartOperationResultDto
                {
                    Success = true,
                    Message = "Cart cleared successfully",
                    Cart = null,
                    Summary = new CartSummaryResponseDto { TotalItems = 0, TotalAmount = 0, CartItemCount = 0 }
                };
            }
            catch (Exception ex)
            {
                return new CartOperationResultDto
                {
                    Success = false,
                    Message = $"Error clearing cart: {ex.Message}"
                };
            }
        }

        public async Task<List<CartDetailResponseDto>> GetCartItemsAsync(int userId)
        {
            var cart = await GetUserCartAsync(userId);
            if (cart == null)
                return new List<CartDetailResponseDto>();

            return cart.CartDetails;
        }

        public async Task<CartSummaryResponseDto> GetCartSummaryAsync(int userId)
        {
            var cart = await GetUserCartAsync(userId);
            if (cart == null)
                return new CartSummaryResponseDto { TotalItems = 0, TotalAmount = 0, CartItemCount = 0 };

            return new CartSummaryResponseDto
            {
                TotalItems = cart.TotalQuantity,
                TotalAmount = cart.AmountDue,
                CartItemCount = cart.CartDetails.Count
            };
        }

        public async Task<bool> ValidateCartItemAsync(int cartDetailId, int userId)
        {
            var cartItem = await _cartRepository.GetCartItemAsync(cartDetailId);
            if (cartItem == null)
                return false;

            var cart = await GetUserCartAsync(userId);
            if (cart == null)
                return false;

            // Check if cart item belongs to the user's cart
            return cartItem.CartId == cart.CartId;
        }

        public async Task<bool> ValidateProductForCartAsync(int productId, int quantity)
        {
            if (quantity <= 0)
                return false;

            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                return false;

            // Check if product is active (not deleted)
            if (product.IsDelete)
                return false;

            // Calculate total stock from variants
            var totalStock = 0;
            if (product.Variants != null)
            {
                foreach (var variant in product.Variants)
                {
                    try
                    {
                        var variantsArray = JsonConvert.DeserializeObject<JArray>(variant.Variants ?? "[]");
                        foreach (var variantItem in variantsArray)
                        {
                            if (variantItem["stock"] != null)
                            {
                                totalStock += variantItem["stock"].Value<int>();
                            }
                        }
                    }
                    catch (JsonException)
                    {
                        // If JSON parsing fails, continue with next variant
                        continue;
                    }
                }
            }
            
            // Check if product has stock
            if (totalStock <= 0)
                return false;

            // Check if requested quantity is available
            if (totalStock < quantity)
                return false;

            return true;
        }

        // Mapping methods
        private CartResponseDto MapToCartResponseDto(Cart cart)
        {
            return new CartResponseDto
            {
                CartId = cart.CartId,
                CustomerId = cart.CustomerId ?? 0,
                TotalQuantity = cart.TotalQuantity ?? 0,
                AmountDue = cart.AmountDue ?? 0,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                CartDetails = cart.CartDetails?.Select(cd => MapToCartDetailResponseDto(cd)).ToList() ?? new List<CartDetailResponseDto>()
            };
        }

        private CartDetailResponseDto MapToCartDetailResponseDto(CartDetail cartDetail)
        {
            return new CartDetailResponseDto
            {
                CartDetailId = cartDetail.CartDetailId,
                ProductId = cartDetail.ProductId ?? 0,
                VariantId = cartDetail.VariantId,
                ProductName = cartDetail.ProductName ?? string.Empty,
                Quantity = cartDetail.Quantity ?? 0,
                Price = cartDetail.Price ?? 0,
                VariantAttributes = cartDetail.VariantAttributes,
                ProductImage = cartDetail.Product?.ProductImages?.FirstOrDefault()?.ImageUrl,
                ProductDescription = cartDetail.Product?.Description
            };
        }
    }
} 