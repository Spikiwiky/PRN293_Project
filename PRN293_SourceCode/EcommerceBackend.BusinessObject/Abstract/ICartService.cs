using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.BusinessObject.dtos.CartDto;

namespace EcommerceBackend.BusinessObject.Abstract
{
    public interface ICartService
    {
        // Cart operations
        Task<CartResponseDto?> GetUserCartAsync(int userId);
        Task<CartResponseDto> CreateUserCartAsync(int userId);
        
        // Cart item operations
        Task<CartOperationResultDto> AddToCartAsync(int userId, AddToCartRequestDto request);
        Task<CartDetailResponseDto?> GetCartItemAsync(int cartDetailId);
        Task<CartOperationResultDto> UpdateCartItemQuantityAsync(int cartDetailId, UpdateCartItemRequestDto request);
        Task<CartOperationResultDto> IncreaseCartItemQuantityAsync(int cartDetailId, IncreaseCartItemRequestDto request);
        Task<CartOperationResultDto> DecreaseCartItemQuantityAsync(int cartDetailId, DecreaseCartItemRequestDto request);
        Task<CartOperationResultDto> RemoveFromCartAsync(int cartDetailId);
        Task<CartOperationResultDto> ClearCartAsync(int userId);
        
        // Cart queries
        Task<List<CartDetailResponseDto>> GetCartItemsAsync(int userId);
        Task<CartSummaryResponseDto> GetCartSummaryAsync(int userId);
        
        // Validation
        Task<bool> ValidateCartItemAsync(int cartDetailId, int userId);
        Task<bool> ValidateProductForCartAsync(int productId, int quantity);
    }
} 