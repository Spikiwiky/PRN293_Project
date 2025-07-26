using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            Console.WriteLine($"GetCartByCustomerIdAsync called with customerId: {customerId}");
            
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
                
            Console.WriteLine($"Database query result: {(cart != null ? "Cart found" : "Cart not found")}");
            if (cart != null)
            {
                Console.WriteLine($"Cart ID: {cart.CartId}, Customer ID: {cart.CustomerId}, Total Quantity: {cart.TotalQuantity}");
            }
            
            return cart;
        }

        public async Task<Cart?> GetCartByUsernameAsync(string username)
        {
            return await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .ThenInclude(p => p.ProductImages)
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.Customer.UserName == username);
        }

        public async Task<Cart> CreateCartAsync(int customerId)
        {
            // For guest users (ID 999), we need to ensure the user exists
            if (customerId == 999)
            {
                // Check if guest user exists, if not create one
                var guestUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == 999);
                if (guestUser == null)
                {
                    // Create guest user if it doesn't exist
                    guestUser = new User
                    {
                        UserId = 999,
                        UserName = "guest_user",
                        Email = "guest@example.com",
                        Password = "guest_password_hash",
                        RoleId = 3, // Assuming 3 is guest role
                        Status = 1,
                      
                    };
                    
                    // Use raw SQL to insert with specific ID
                    await _context.Database.ExecuteSqlRawAsync(@"
                        SET IDENTITY_INSERT [User] ON;
                        INSERT INTO [User] ([User_id], [User_name], [Email], [Password], [Role_id], [Status])
                        VALUES (999, 'guest_user', 'guest@example.com', 'guest_password_hash', 3, 1);
                        SET IDENTITY_INSERT [User] OFF;
                    ");
                }
            }

            var cart = new Cart
            {
                CustomerId = customerId,
                TotalQuantity = 0,
                AmountDue = 0
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> CreateCartByUsernameAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
                throw new ArgumentException($"User with username '{username}' not found");

            var cart = new Cart
            {
                CustomerId = user.UserId,
                TotalQuantity = 0,
                AmountDue = 0
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<bool> CartExistsAsync(int cartId)
        {
            return await _context.Carts.AnyAsync(c => c.CartId == cartId);
        }

        public async Task<CartDetail> AddItemToCartAsync(int cartId, int productId, int? variantId, string? variantAttributes, int quantity)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Variants)
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

            // Calculate price based on variant if available
            decimal price = product.BasePrice;
            if (variantId.HasValue && product.Variants != null)
            {
                var variant = product.Variants.FirstOrDefault(v => v.VariantId == variantId.Value);
                if (variant != null)
                {
                    try
                    {
                        var variantsArray = JsonConvert.DeserializeObject<JArray>(variant.Variants ?? "[]");
                        foreach (var variantItem in variantsArray)
                        {
                            if (variantItem["price"] != null)
                            {
                                price = variantItem["price"].Value<decimal>();
                                break; // Use the first price found
                            }
                        }
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        // If JSON parsing fails, use base price
                        price = product.BasePrice;
                    }
                }
            }

            // Create new cart item
            var cartItem = new CartDetail
            {
                CartId = cartId,
                ProductId = productId,
                VariantId = variantId,
                ProductName = product.Name,
                Quantity = quantity,
                Price = price,
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
            var cartItem = await _context.CartDetails
                .Include(cd => cd.Cart)
                .FirstOrDefaultAsync(cd => cd.CartDetailId == cartDetailId);
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
            var cartItem = await _context.CartDetails
                .Include(cd => cd.Cart)
                .FirstOrDefaultAsync(cd => cd.CartDetailId == cartDetailId);
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
            var cartItem = await _context.CartDetails
                .Include(cd => cd.Cart)
                .FirstOrDefaultAsync(cd => cd.CartDetailId == cartDetailId);
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

      
        public async Task<bool> CartItemExistsAsync(int cartId, int productId, int? variantId = null, string? variantAttributes = null)
        {
            var query = _context.CartDetails
                .Where(cd => cd.CartId == cartId && cd.ProductId == productId);

            // Nếu cả variantId và variantAttributes đều được cung cấp, kiểm tra cả hai
            if (variantId.HasValue && !string.IsNullOrEmpty(variantAttributes))
            {
                var normalizedVariantAttributes = variantAttributes.Trim();
                // Use client-side evaluation for JSON comparison
                var cartItems = await query.Where(cd => cd.VariantId == variantId.Value).ToListAsync();
                return cartItems.Any(cd => AreVariantAttributesEqual(cd.VariantAttributes, normalizedVariantAttributes));
            }
            // Nếu chỉ có variantId, kiểm tra theo variantId
            else if (variantId.HasValue)
            {
                return await query.Where(cd => cd.VariantId == variantId.Value).AnyAsync();
            }
            // Nếu chỉ có variantAttributes, kiểm tra theo variantAttributes
            else if (!string.IsNullOrEmpty(variantAttributes))
            {
                var normalizedVariantAttributes = variantAttributes.Trim();
                // Use client-side evaluation for JSON comparison
                var cartItems = await query.ToListAsync();
                return cartItems.Any(cd => AreVariantAttributesEqual(cd.VariantAttributes, normalizedVariantAttributes));
            }

            return await query.AnyAsync();
        }

   
        public async Task<CartDetail?> GetCartItemByProductAndVariantAsync(int cartId, int productId, int? variantId = null, string? variantAttributes = null)
        {
            var query = _context.CartDetails
                .Include(cd => cd.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(cd => cd.CartId == cartId && cd.ProductId == productId);

            // Nếu cả variantId và variantAttributes đều được cung cấp, kiểm tra cả hai
            if (variantId.HasValue && !string.IsNullOrEmpty(variantAttributes))
            {
                var normalizedVariantAttributes = variantAttributes.Trim();
                // Use client-side evaluation for JSON comparison
                var cartItems = await query.Where(cd => cd.VariantId == variantId.Value).ToListAsync();
                return cartItems.FirstOrDefault(cd => AreVariantAttributesEqual(cd.VariantAttributes, normalizedVariantAttributes));
            }
            // Nếu chỉ có variantId, kiểm tra theo variantId
            else if (variantId.HasValue)
            {
                return await query.Where(cd => cd.VariantId == variantId.Value).FirstOrDefaultAsync();
            }
            // Nếu chỉ có variantAttributes, kiểm tra theo variantAttributes
            else if (!string.IsNullOrEmpty(variantAttributes))
            {
                var normalizedVariantAttributes = variantAttributes.Trim();
                // Use client-side evaluation for JSON comparison
                var cartItems = await query.ToListAsync();
                return cartItems.FirstOrDefault(cd => AreVariantAttributesEqual(cd.VariantAttributes, normalizedVariantAttributes));
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

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Chuẩn hóa và so sánh variant attributes
        /// </summary>
        /// <param name="attributes1">Variant attributes thứ nhất</param>
        /// <param name="attributes2">Variant attributes thứ hai</param>
        /// <returns>True nếu hai attributes giống nhau</returns>
        private bool AreVariantAttributesEqual(string? attributes1, string? attributes2)
        {
            if (string.IsNullOrEmpty(attributes1) && string.IsNullOrEmpty(attributes2)) return true;
            if (string.IsNullOrEmpty(attributes1) || string.IsNullOrEmpty(attributes2)) return false;
            
            try
            {
                // Parse JSON objects
                var objA = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(attributes1);
                var objB = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(attributes2);
                
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
            catch (Exception)
            {
                // Fallback to string comparison if JSON parsing fails
                var normalized1 = attributes1?.Trim() ?? "";
                var normalized2 = attributes2?.Trim() ?? "";
                return string.Equals(normalized1, normalized2, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
} 