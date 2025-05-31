// EcommerceBackend.DataAccess.Repository/ProductRepository.cs
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

        public async Task<List<Product>> SearchProductsAsync(string name, string category, string size, string color, int page, int pageSize)
        {
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

           
            if (!string.IsNullOrEmpty(size) || !string.IsNullOrEmpty(color))
            {
                products = products
                    .Where(p =>
                    {
                        var variants = JsonDocument.Parse(p.Variants).RootElement;
                        var matchesSize = string.IsNullOrEmpty(size) || variants.GetProperty("size").GetString().Contains(size);
                        var matchesColor = string.IsNullOrEmpty(color) || variants.GetProperty("color").GetString().Contains(color);
                        return matchesSize && matchesColor;
                    })
                    .ToList();
            }

            return products;
        }
    }
}