using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EcommerceBackend.DataAccess.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceDBContext _context;

        public ProductRepository(EcommerceDBContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync(int page, int pageSize)
        {
            return await _context.Products
                .Where(p => (p.IsDelete ?? false) == false)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImages)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchProductsAsync(string name = null, string category = null, string size = null, string color = null, string variantId = null, decimal? price = null, int page = 1, int pageSize = 10)
        {
            if (page < 1) throw new ArgumentException("Page must be greater than or equal to 1.");
            if (pageSize <= 0) throw new ArgumentException("Page size must be greater than 0.");

            var query = _context.Products
                .Where(p => (p.IsDelete ?? false) == false);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.ProductName != null && p.ProductName.ToLower().Contains(name.ToLower()));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.ProductCategory != null &&
                                        p.ProductCategory.ProductCategoryTitle != null &&
                                        p.ProductCategory.ProductCategoryTitle.ToLower().Contains(category.ToLower()));
            }

            var products = await query
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImages)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!string.IsNullOrEmpty(size) || !string.IsNullOrEmpty(color) || !string.IsNullOrEmpty(variantId) || price.HasValue)
            {
                products = products
                    .Where(p =>
                    {
                        if (string.IsNullOrEmpty(p.Variants)) return false;

                        var variants = JsonNode.Parse(p.Variants).AsObject();
                        var matchesSize = string.IsNullOrEmpty(size) || variants["size"]?.GetValue<string>()?.Contains(size) == true;
                        var matchesColor = string.IsNullOrEmpty(color) || variants["color"]?.GetValue<string>()?.Contains(color) == true;
                        var matchesVariantId = string.IsNullOrEmpty(variantId) || variants["variant_id"]?.GetValue<string>()?.Contains(variantId) == true;
                        var matchesPrice = !price.HasValue || variants["price"]?.GetValue<decimal>() == price;

                        return matchesSize && matchesColor && matchesVariantId && matchesPrice;
                    })
                    .ToList();
            }

            return products;
        }
    }
}