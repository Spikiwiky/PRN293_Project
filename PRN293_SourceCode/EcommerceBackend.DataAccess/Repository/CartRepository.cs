using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly EcommerceDBContext _context;
        public CartRepository(EcommerceDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts.Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);
        }

        public async Task AddOrUpdateCartItemAsync(int userId, int productId, int quantity)
        {
            var cart = await _context.Carts.Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);
            if (cart == null)
            {
                cart = new Cart { CustomerId = userId, TotalQuantity = 0, AmountDue = 0 };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }
            var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId);
            if (cartDetail == null && quantity > 0)
            {
                // Lấy thông tin sản phẩm
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                cartDetail = new CartDetail {
                    ProductId = productId,
                    Quantity = quantity,
                    CartId = cart.CartId,
                    ProductName = product?.Name,
                    Price = product?.BasePrice
                };
                _context.CartDetails.Add(cartDetail);
            }
            else if (cartDetail != null)
            {
                if (quantity > 0)
                {
                    // Nếu quantity == cartDetail.Quantity thì không làm gì
                    if (quantity == cartDetail.Quantity) { /* Không thay đổi */ }
                    // Nếu quantity > cartDetail.Quantity thì cộng dồn (AddToCart)
                    else if (quantity > cartDetail.Quantity) {
                        cartDetail.Quantity += (quantity - cartDetail.Quantity);
                        _context.CartDetails.Update(cartDetail);
                    }
                    // Nếu quantity < cartDetail.Quantity thì set lại (UpdateCart)
                    else {
                        cartDetail.Quantity = quantity;
                        _context.CartDetails.Update(cartDetail);
                    }
                }
                else
                {
                    _context.CartDetails.Remove(cartDetail);
                }
            }
            cart.TotalQuantity = cart.CartDetails.Where(cd => cd.Quantity > 0).Sum(cd => cd.Quantity ?? 0);
            cart.AmountDue = cart.CartDetails.Where(cd => cd.Quantity > 0).Sum(cd => (cd.Price ?? 0) * (cd.Quantity ?? 0));
            await _context.SaveChangesAsync();
        }

        public async Task AddToCartAsync(int userId, int productId, int variantId, int quantity, string variantAttributes)
        {
            var cart = await _context.Carts.Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);
            if (cart == null)
            {
                cart = new Cart { CustomerId = userId, TotalQuantity = 0, AmountDue = 0 };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }
            var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId && cd.VariantId == variantId.ToString() && cd.VariantAttributes == variantAttributes);
            if (cartDetail == null)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                cartDetail = new CartDetail {
                    ProductId = productId,
                    VariantId = variantId.ToString(),
                    VariantAttributes = variantAttributes,
                    Quantity = quantity,
                    CartId = cart.CartId,
                    ProductName = product?.Name,
                    Price = product?.BasePrice
                };
                _context.CartDetails.Add(cartDetail);
            }
            else
            {
                // Cộng dồn đúng số lượng client gửi lên
                cartDetail.Quantity = (cartDetail.Quantity ?? 0) + quantity;
                _context.CartDetails.Update(cartDetail);
            }
            cart.TotalQuantity = cart.CartDetails.Where(cd => cd.Quantity > 0).Sum(cd => cd.Quantity ?? 0);
            cart.AmountDue = cart.CartDetails.Where(cd => cd.Quantity > 0).Sum(cd => (cd.Price ?? 0) * (cd.Quantity ?? 0));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(int userId, int productId, int variantId, int quantity, string variantAttributes)
        {
            var cart = await _context.Carts.Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);
            if (cart == null) return;
            var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId && cd.VariantId == variantId.ToString() && cd.VariantAttributes == variantAttributes);
            if (cartDetail != null)
            {
                if (quantity > 0)
                {
                    cartDetail.Quantity = quantity;
                    _context.CartDetails.Update(cartDetail);
                }
                else
                {
                    _context.CartDetails.Remove(cartDetail);
                }
                cart.TotalQuantity = cart.CartDetails.Where(cd => cd.Quantity > 0).Sum(cd => cd.Quantity ?? 0);
                cart.AmountDue = cart.CartDetails.Where(cd => cd.Quantity > 0).Sum(cd => (cd.Price ?? 0) * (cd.Quantity ?? 0));
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _context.Carts.Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);
            if (cart != null)
            {
                _context.CartDetails.RemoveRange(cart.CartDetails);
                cart.TotalQuantity = 0;
                cart.AmountDue = 0;
                await _context.SaveChangesAsync();
            }
        }
    }
} 