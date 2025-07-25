using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBackend.DataAccess.Repository.CartRepository
{
    public class CartRepository : ICartRepository
    {
        private readonly EcommerceDBContext _context;

        public CartRepository(EcommerceDBContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByCustomerIdAsync(int customerId)
        {
            return await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<Cart> CreateCartAsync(int customerId)
        {
            var cart = new Cart
            {
                CustomerId = customerId,
                TotalQuantity = 0,
                AmountDue = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<bool> CartExistsAsync(int cartId)
        {
            return await _context.Carts.AnyAsync(c => c.CartId == cartId);
        }

        public async Task<CartDetail> AddItemToCartAsync(int cartId, int productId, string? variantId, string? variantAttributes, int quantity)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                throw new ArgumentException("Product not found");

            // Check if item already exists in cart
            var existingItem = await GetCartItemByProductAndVariantAsync(cartId, productId, variantId, variantAttributes);
            
            if (existingItem != null)
            {
                // If item exists, increase quantity
                existingItem.Quantity = (existingItem.Quantity ?? 0) + quantity;
               
                await _context.SaveChangesAsync();

                // Update cart totals
                await UpdateCartTotalsAsync(cartId);

                return existingItem;
            }

            // Create new cart item
            var cartItem = new CartDetail
            {
                CartId = cartId,
                ProductId = productId,
                VariantId = variantId,
                ProductName = product.Name,
                Quantity = quantity,
                Price = product.BasePrice,
                VariantAttributes = variantAttributes,
                
            };

            _context.CartDetails.Add(cartItem);
            await _context.SaveChangesAsync();

            // Update cart totals
            await UpdateCartTotalsAsync(cartId);

            return cartItem;
        }

        public async Task<CartDetail?> GetCartItemAsync(int cartDetailId)
        {
            return await _context.CartDetails
                .Include(cd => cd.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(cd => cd.CartDetailId == cartDetailId);
        }

        public async Task<CartDetail?> UpdateCartItemAsync(int cartDetailId, int quantity)
        {
            var cartItem = await _context.CartDetails.FindAsync(cartDetailId);
            if (cartItem == null)
                return null;

            cartItem.Quantity = quantity;
            
            await _context.SaveChangesAsync();

            // Update cart totals
            await UpdateCartTotalsAsync(cartItem.CartId.Value);

            return cartItem;
        }

        public async Task<CartDetail?> IncreaseCartItemQuantityAsync(int cartDetailId, int quantityToAdd = 1)
        {
            var cartItem = await _context.CartDetails.FindAsync(cartDetailId);
            if (cartItem == null)
                return null;

            cartItem.Quantity = (cartItem.Quantity ?? 0) + quantityToAdd;
          
            await _context.SaveChangesAsync();

            // Update cart totals
            await UpdateCartTotalsAsync(cartItem.CartId.Value);

            return cartItem;
        }

        public async Task<CartDetail?> DecreaseCartItemQuantityAsync(int cartDetailId, int quantityToRemove = 1)
        {
            var cartItem = await _context.CartDetails.FindAsync(cartDetailId);
            if (cartItem == null)
                return null;

            var currentQuantity = cartItem.Quantity ?? 0;
            var newQuantity = currentQuantity - quantityToRemove;

            // If quantity becomes 0 or negative, remove the item
            if (newQuantity <= 0)
            {
                return await RemoveCartItemAsync(cartDetailId) ? null : cartItem;
            }

            cartItem.Quantity = newQuantity;
            
            await _context.SaveChangesAsync();

            // Update cart totals
            await UpdateCartTotalsAsync(cartItem.CartId.Value);

            return cartItem;
        }

        public async Task<bool> RemoveCartItemAsync(int cartDetailId)
        {
            var cartItem = await _context.CartDetails.FindAsync(cartDetailId);
            if (cartItem == null)
                return false;

            var cartId = cartItem.CartId;
            _context.CartDetails.Remove(cartItem);
            await _context.SaveChangesAsync();

            // Update cart totals
            await UpdateCartTotalsAsync(cartId.Value);

            return true;
        }

        public async Task<bool> ClearCartAsync(int cartId)
        {
            var cartItems = await _context.CartDetails
                .Where(cd => cd.CartId == cartId)
                .ToListAsync();

            if (!cartItems.Any())
                return false;

            _context.CartDetails.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // Update cart totals
            await UpdateCartTotalsAsync(cartId);

            return true;
        }

        public async Task<List<CartDetail>> GetCartItemsAsync(int cartId)
        {
            return await _context.CartDetails
                .Include(cd => cd.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(cd => cd.CartId == cartId)
                .ToListAsync();
        }

        public async Task<bool> CartItemExistsAsync(int cartId, int productId, string? variantId = null, string? variantAttributes = null)
        {
            var query = _context.CartDetails
                .Where(cd => cd.CartId == cartId && cd.ProductId == productId);

            // If variantId is provided, check by variantId
            if (!string.IsNullOrEmpty(variantId))
            {
                query = query.Where(cd => cd.VariantId == variantId);
            }
            // If variantAttributes is provided, check by variantAttributes
            else if (!string.IsNullOrEmpty(variantAttributes))
            {
                query = query.Where(cd => cd.VariantAttributes == variantAttributes);
            }

            return await query.AnyAsync();
        }

        public async Task<CartDetail?> GetCartItemByProductAndVariantAsync(int cartId, int productId, string? variantId = null, string? variantAttributes = null)
        {
            var query = _context.CartDetails
                .Include(cd => cd.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(cd => cd.CartId == cartId && cd.ProductId == productId);

            // If variantId is provided, check by variantId
            if (!string.IsNullOrEmpty(variantId))
            {
                query = query.Where(cd => cd.VariantId == variantId);
            }
            // If variantAttributes is provided, check by variantAttributes
            else if (!string.IsNullOrEmpty(variantAttributes))
            {
                query = query.Where(cd => cd.VariantAttributes == variantAttributes);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<(int TotalItems, decimal TotalAmount)> GetCartSummaryAsync(int cartId)
        {
            var cartItems = await _context.CartDetails
                .Where(cd => cd.CartId == cartId)
                .ToListAsync();

            var totalItems = cartItems.Sum(cd => cd.Quantity ?? 0);
            var totalAmount = cartItems.Sum(cd => (cd.Price ?? 0) * (cd.Quantity ?? 0));

            return (totalItems, totalAmount);
        }

        private async Task UpdateCartTotalsAsync(int cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart == null)
                return;

            var (totalItems, totalAmount) = await GetCartSummaryAsync(cartId);

            cart.TotalQuantity = totalItems;
            cart.AmountDue = totalAmount;
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
} 