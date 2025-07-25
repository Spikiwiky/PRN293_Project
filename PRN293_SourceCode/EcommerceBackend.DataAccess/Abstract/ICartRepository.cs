using EcommerceBackend.DataAccess.Models;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Abstract
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart> GetCartByUserIdAsync(int userId);
        Task AddOrUpdateCartItemAsync(int userId, int productId, int quantity);
        Task AddToCartAsync(int userId, int productId, int variantId, int quantity, string variantAttributes);
        Task UpdateCartItemAsync(int userId, int productId, int variantId, int quantity, string variantAttributes);
        Task ClearCartAsync(int userId);
    }
} 