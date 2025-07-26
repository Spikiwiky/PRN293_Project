using EcommerceBackend.BusinessObject.Abstract;
using EcommerceBackend.BusinessObject.dtos.CartDto;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Abstract.AuthAbstract;
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
        private readonly IAuthRepository _authRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository, IAuthRepository authRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _authRepository = authRepository;
        }

        public async Task<CartResponseDto?> GetUserCartAsync(int userId)
        {
            Console.WriteLine($"GetUserCartAsync called with userId: {userId}");
            
            var cart = await _cartRepository.GetCartByCustomerIdAsync(userId);
            Console.WriteLine($"CartRepository returned: {(cart != null ? "Cart found" : "Cart not found")}");
            
            if (cart == null)
                return null;

            return MapToCartResponseDto(cart);
        }

        public async Task<CartResponseDto?> GetUserCartByUsernameAsync(string username)
        {
            var cart = await _cartRepository.GetCartByUsernameAsync(username);
            if (cart == null)
                return null;

            return MapToCartResponseDto(cart);
        }

        public async Task<CartResponseDto> CreateUserCartAsync(int userId)
        {
            // For guest users (ID 999), skip user validation
            if (userId != 999)
            {
                // Validate that user exists
                var user = await _authRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {userId} does not exist in the database.");
                }
            }

            // Check if user already has a cart
            var existingCart = await _cartRepository.GetCartByCustomerIdAsync(userId);
            if (existingCart != null)
                return MapToCartResponseDto(existingCart);

            var cart = await _cartRepository.CreateCartAsync(userId);
            return MapToCartResponseDto(cart);
        }

        public async Task<CartResponseDto> CreateUserCartByUsernameAsync(string username)
        {
            // Validate that user exists
            var user = await _authRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                throw new InvalidOperationException($"User with username '{username}' does not exist in the database.");
            }

            // Check if user already has a cart
            var existingCart = await _cartRepository.GetCartByUsernameAsync(username);
            if (existingCart != null)
                return MapToCartResponseDto(existingCart);

            var cart = await _cartRepository.CreateCartByUsernameAsync(username);
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

        public async Task<CartOperationResultDto> AddToCartAsync(string username, AddToCartRequestDto request)
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
                var cart = await GetUserCartByUsernameAsync(username) ?? await CreateUserCartByUsernameAsync(username);

                // Add item to cart
                var cartItem = await _cartRepository.AddItemToCartAsync(
                    cart.CartId, 
                    request.ProductId, 
                    request.VariantId, 
                    request.VariantAttributes, 
                    request.Quantity
                );

                var updatedCart = await GetUserCartByUsernameAsync(username);
                var summary = await GetCartSummaryAsync(cart.CustomerId);

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

        public async Task<List<CartItemDto>> GetCartItemsByUserIdAsync(int userId)
        {
            var cartItems = await GetCartItemsAsync(userId);
            return cartItems.Select(item => new CartItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName ?? string.Empty,
                ProductImage = item.ProductImage ?? string.Empty,
                Quantity = item.Quantity,
                Price = item.Price,
                VariantAttributes = item.VariantAttributes ?? string.Empty,
                CartDetailId = item.CartDetailId
            }).ToList();
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
                CreatedAt = DateTime.UtcNow, // Use current time since the column doesn't exist in DB
                UpdatedAt = DateTime.UtcNow, // Use current time since the column doesn't exist in DB
                CartDetails = cart.CartDetails?.Select(cd => MapToCartDetailResponseDto(cd)).ToList() ?? new List<CartDetailResponseDto>()
            };
        }

        private CartDetailResponseDto MapToCartDetailResponseDto(CartDetail cartDetail)
        {
            // Get product image - try multiple sources
            string? productImage = null;
            if (cartDetail.Product?.ProductImages?.Any() == true)
            {
                var imageUrl = cartDetail.Product.ProductImages.FirstOrDefault()?.ImageUrl;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    // Ensure the image URL has the correct path
                    if (imageUrl.StartsWith("/"))
                    {
                        productImage = imageUrl;
                    }
                    else
                    {
                        productImage = "/images/products/" + imageUrl;
                    }
                }
            }
            
            // If no image from ProductImages, try to construct from product name or use default
            if (string.IsNullOrEmpty(productImage))
            {
                if (cartDetail.ProductId.HasValue)
                {
                    // Try to construct image path based on product ID
                    productImage = $"/images/products/product-{cartDetail.ProductId:D2}.jpg";
                }
                else
                {
                    productImage = "/images/default-product.jpg";
                }
            }

            return new CartDetailResponseDto
            {
                CartDetailId = cartDetail.CartDetailId,
                ProductId = cartDetail.ProductId ?? 0,
                VariantId = cartDetail.VariantId,
                ProductName = cartDetail.ProductName ?? string.Empty,
                Quantity = cartDetail.Quantity ?? 0,
                Price = cartDetail.Price ?? 0,
                VariantAttributes = cartDetail.VariantAttributes,
                ProductImage = productImage,
                ProductDescription = cartDetail.Product?.Description
            };
        }
    }
} 