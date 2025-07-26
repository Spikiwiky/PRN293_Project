using EcommerceBackend.DataAccess.Models;

namespace EcommerceBackend.DataAccess.Abstract
{
    public interface ICartRepository
    {
        // Cart operations
        Task<Cart?> GetCartByCustomerIdAsync(int customerId);
        Task<Cart?> GetCartByUsernameAsync(string username);
        Task<Cart> CreateCartAsync(int customerId);
        Task<Cart> CreateCartByUsernameAsync(string username);
        Task<bool> CartExistsAsync(int cartId);
        
        // Cart item operations
        Task<CartDetail> AddItemToCartAsync(int cartId, int productId, int? variantId, string? variantAttributes, int quantity);
        Task<CartDetail?> GetCartItemAsync(int cartDetailId);
        Task<CartDetail?> UpdateCartItemAsync(int cartDetailId, int quantity);
        Task<CartDetail?> IncreaseCartItemQuantityAsync(int cartDetailId, int quantityToAdd = 1);
        Task<CartDetail?> DecreaseCartItemQuantityAsync(int cartDetailId, int quantityToRemove = 1);
        Task<bool> RemoveCartItemAsync(int cartDetailId);
        Task<bool> ClearCartAsync(int cartId);
        
        // Cart item queries
        Task<List<CartDetail>> GetCartItemsAsync(int cartId);
        Task<bool> CartItemExistsAsync(int cartId, int productId, int? variantId = null, string? variantAttributes = null);
        Task<CartDetail?> GetCartItemByProductAndVariantAsync(int cartId, int productId, int? variantId = null, string? variantAttributes = null);
        
        // Cart summary
        Task<(int TotalItems, decimal TotalAmount)> GetCartSummaryAsync(int cartId);
    }
} 