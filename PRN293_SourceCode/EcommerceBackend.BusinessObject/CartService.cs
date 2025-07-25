using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services
{
    public class CartService 
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task AddToCart(int userId, int productId, int variantId, int quantity, string variantAttributes)
        {
            await _cartRepository.AddToCartAsync(userId, productId, variantId, quantity, variantAttributes);
        }
        public async Task UpdateCartItem(int userId, int productId, int variantId, int quantity, string variantAttributes)
        {
            await _cartRepository.UpdateCartItemAsync(userId, productId, variantId, quantity, variantAttributes);
        }

        public async Task<Cart> GetCartByUserId(int userId)
        {
            return await _cartRepository.GetCartByUserIdAsync(userId);
        }

        public async Task ClearCart(int userId)
        {
            await _cartRepository.ClearCartAsync(userId);
        }
    }
} 