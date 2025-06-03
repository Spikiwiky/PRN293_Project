using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcommerceBackend.DataAccess.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceDBContext _context;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductRepository(EcommerceDBContext context)
        {
            _context = context;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        private bool HasValidVariants(string? variants)
        {
            if (string.IsNullOrEmpty(variants))
                return false;

            try
            {
                var variantsList = JsonSerializer.Deserialize<List<object>>(variants, _jsonOptions);
                return variantsList?.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Product>> GetAllProductsAsync(int page, int pageSize)
        {
            return await _context.Products
                .Where(p => p.IsDelete != true)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImages)
                .OrderByDescending(p => p.ProductId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchProductsAsync(
            string? name = null,
            string? category = null,
            string? size = null,
            string? color = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 10)
        {
            if (page < 1) throw new ArgumentException("Page must be greater than or equal to 1.");
            if (pageSize <= 0) throw new ArgumentException("Page size must be greater than 0.");

            var query = _context.Products
                .Where(p => p.IsDelete != true);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.ProductName.Contains(name));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.ProductCategory.ProductCategoryTitle.Contains(category));
            }

            var products = await query
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImages)
                .ToListAsync();

            // Filter by variant properties
            if (!string.IsNullOrEmpty(size) || !string.IsNullOrEmpty(color) ||
                minPrice.HasValue || maxPrice.HasValue)
            {
                products = products.Where(p =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(p.Variants)) return false;

                        var variants = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(p.Variants, _jsonOptions);
                        if (variants == null) return false;

                        return variants.Any(variant =>
                        {
                            var variantSize = variant.GetValueOrDefault("size").GetString();
                            var variantColor = variant.GetValueOrDefault("color").GetString();
                            var variantPrice = variant.GetValueOrDefault("price").GetDecimal();

                            var matchesSize = string.IsNullOrEmpty(size) ||
                                (variantSize?.Equals(size, StringComparison.OrdinalIgnoreCase) ?? false);

                            var matchesColor = string.IsNullOrEmpty(color) ||
                                (variantColor?.Equals(color, StringComparison.OrdinalIgnoreCase) ?? false);

                            var matchesPrice = (!minPrice.HasValue || variantPrice >= minPrice.Value) &&
                                             (!maxPrice.HasValue || variantPrice <= maxPrice.Value);

                            return matchesSize && matchesColor && matchesPrice;
                        });
                    }
                    catch
                    {
                        return false;
                    }
                }).ToList();
            }

            return products
                .OrderByDescending(p => p.ProductId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}