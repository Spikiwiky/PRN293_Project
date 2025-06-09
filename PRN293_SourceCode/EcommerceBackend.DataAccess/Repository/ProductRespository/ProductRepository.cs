using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace EcommerceBackend.DataAccess.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceDBContext _context;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(EcommerceDBContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<Product>> GetAllProductsAsync(int page, int pageSize)
        {
            return await _context.Products
                .Where(p => !p.IsDelete)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImages)
                .Include(p => p.Variants)
                .OrderByDescending(p => p.ProductId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImages)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.ProductId == productId && !p.IsDelete);
        }

        public async Task<List<Product>> SearchProductsAsync(
      string? name = null,
      string? category = null,
      Dictionary<string, string>? attributes = null,
      decimal? minPrice = null,
      decimal? maxPrice = null,
      int page = 1,
      int pageSize = 10)
        {
            var query = _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Variants)
                .Where(p => !p.IsDelete);

            // Lọc theo tên sản phẩm
            if (!string.IsNullOrEmpty(name))
            {
                var lowerName = name.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(lowerName));
            }

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(category))
            {
                var lowerCategory = category.ToLower();
                query = query.Where(p => p.ProductCategory != null &&
                    p.ProductCategory.ProductCategoryTitle.ToLower().Contains(lowerCategory));
            }

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.BasePrice != null && p.BasePrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.BasePrice != null && p.BasePrice <= maxPrice.Value);
            }

            // Phân trang
            query = query.OrderByDescending(p => p.ProductId)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize);

            // Thực thi truy vấn
            var products = await query.ToListAsync();

            // Lọc theo attributes nếu có
            //if (attributes != null && attributes.Any())
            //{
            //    products = products.Where(p => p.Variants.Any(variant =>
            //    {
            //        try
            //        {
            //            if (string.IsNullOrEmpty(variant.Attributes)) return false;
            //            var variantAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(variant.Attributes);
            //            return attributes.All(attr =>
            //            {
            //                if (!variantAttributes.TryGetValue(attr.Key, out var value))
            //                {
            //                    // Nếu khóa không tồn tại, chấp nhận nếu giá trị trong attributes là null hoặc rỗng
            //                    return false;
            //                }
            //                // So sánh giá trị, chấp nhận nếu cả hai đều null hoặc giá trị khớp
            //                return value == attr.Value || (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(attr.Value));
            //            });
            //        }
            //        catch (JsonException)
            //        {
            //            return false;
            //        }
            //    })).ToList();
            //}

            return products;
        }

        // Hàm kiểm tra JSON hợp lệ
        private bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput))
                return false;
            try
            {
                System.Text.Json.JsonDocument.Parse(strInput);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProductAttributesAsync(int productId, string availableAttributes)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null) return false;

                product.AvailableAttributes = availableAttributes;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddProductVariantAsync(ProductVariant variant)
        {
            try
            {
                // Validate that the product exists
                var product = await _context.Products
                    .Include(p => p.Variants)
                    .FirstOrDefaultAsync(p => p.ProductId == variant.ProductId && !p.IsDelete);

                if (product == null) return false;

                // Validate variant attributes against product's available attributes
                var availableAttributes = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                    product.AvailableAttributes, _jsonOptions);

                if (availableAttributes != null)
                {
                    var variantAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(
                        variant.Attributes, _jsonOptions);

                    if (variantAttributes != null)
                    {
                        foreach (var attr in variantAttributes)
                        {
                            if (!availableAttributes.ContainsKey(attr.Key) ||
                                !availableAttributes[attr.Key].Contains(attr.Value))
                            {
                                return false;
                            }
                        }
                    }
                }

                variant.CreatedAt = DateTime.UtcNow;
                variant.UpdatedAt = DateTime.UtcNow;

                await _context.ProductVariants.AddAsync(variant);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProductVariantAsync(ProductVariant variant)
        {
            try
            {
                var existingVariant = await _context.ProductVariants
                    .Include(v => v.Product)
                    .FirstOrDefaultAsync(v => v.VariantId == variant.VariantId);

                if (existingVariant == null) return false;

                // Validate variant attributes against product's available attributes
                var availableAttributes = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                    existingVariant.Product.AvailableAttributes, _jsonOptions);

                if (availableAttributes != null)
                {
                    var variantAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(
                        variant.Attributes, _jsonOptions);

                    if (variantAttributes != null)
                    {
                        foreach (var attr in variantAttributes)
                        {
                            if (!availableAttributes.ContainsKey(attr.Key) ||
                                !availableAttributes[attr.Key].Contains(attr.Value))
                            {
                                return false;
                            }
                        }
                    }
                }

                existingVariant.Attributes = variant.Attributes;
                existingVariant.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProductVariantAsync(int variantId)
        {
            try
            {
                var variant = await _context.ProductVariants.FindAsync(variantId);
                if (variant == null) return false;

                _context.ProductVariants.Remove(variant);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddProductAttributeAsync(int productId, string attributeName, List<string> attributeValues)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null) return false;

                var availableAttributes = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                    product.AvailableAttributes, _jsonOptions) ?? new Dictionary<string, List<string>>();

                if (availableAttributes.ContainsKey(attributeName))
                {
                    return false; // Attribute already exists
                }

                availableAttributes[attributeName] = attributeValues;
                product.AvailableAttributes = JsonSerializer.Serialize(availableAttributes, _jsonOptions);
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProductAttributeAsync(int productId, string attributeName, List<string> attributeValues)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null) return false;

                var availableAttributes = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                    product.AvailableAttributes, _jsonOptions) ?? new Dictionary<string, List<string>>();

                if (!availableAttributes.ContainsKey(attributeName))
                {
                    return false; // Attribute doesn't exist
                }

                availableAttributes[attributeName] = attributeValues;
                product.AvailableAttributes = JsonSerializer.Serialize(availableAttributes, _jsonOptions);
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProductAttributeAsync(int productId, string attributeName)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null) return false;

                var availableAttributes = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                    product.AvailableAttributes, _jsonOptions) ?? new Dictionary<string, List<string>>();

                if (!availableAttributes.ContainsKey(attributeName))
                {
                    return false; // Attribute doesn't exist
                }

                availableAttributes.Remove(attributeName);
                product.AvailableAttributes = JsonSerializer.Serialize(availableAttributes, _jsonOptions);
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Dictionary<string, List<string>>> GetProductAttributesAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null) return new Dictionary<string, List<string>>();

                return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                    product.AvailableAttributes, _jsonOptions) ?? new Dictionary<string, List<string>>();
            }
            catch
            {
                return new Dictionary<string, List<string>>();
            }
        }

        public async Task<bool> AddVariantValueAsync(int variantId, Dictionary<string, string> variantValue)
        {
            try
            {
                var variant = await _context.ProductVariants
                    .Include(v => v.Product)
                    .FirstOrDefaultAsync(v => v.VariantId == variantId);

                if (variant == null) return false;

                // Get current variants
                var variants = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(
                    variant.Variants, _jsonOptions) ?? new List<Dictionary<string, string>>();

                // Validate variant structure
                var attributes = JsonSerializer.Deserialize<Dictionary<string, string>>(
                    variant.Attributes, _jsonOptions) ?? new Dictionary<string, string>();

                if (!AreDictionariesEqual(attributes, variantValue))
                {
                    return false; // Structure doesn't match
                }

                // Add new variant value
                variants.Add(variantValue);
                variant.Variants = JsonSerializer.Serialize(variants, _jsonOptions);
                variant.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateVariantValueAsync(int variantId, int valueIndex, Dictionary<string, string> variantValue)
        {
            try
            {
                var variant = await _context.ProductVariants
                    .Include(v => v.Product)
                    .FirstOrDefaultAsync(v => v.VariantId == variantId);

                if (variant == null) return false;

                // Get current variants
                var variants = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(
                    variant.Variants, _jsonOptions) ?? new List<Dictionary<string, string>>();

                if (valueIndex < 0 || valueIndex >= variants.Count)
                {
                    return false; // Invalid index
                }

                // Validate variant structure
                var attributes = JsonSerializer.Deserialize<Dictionary<string, string>>(
                    variant.Attributes, _jsonOptions) ?? new Dictionary<string, string>();

                if (!AreDictionariesEqual(attributes, variantValue))
                {
                    return false; // Structure doesn't match
                }

                // Update variant value
                variants[valueIndex] = variantValue;
                variant.Variants = JsonSerializer.Serialize(variants, _jsonOptions);
                variant.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteVariantValueAsync(int variantId, int valueIndex)
        {
            try
            {
                var variant = await _context.ProductVariants.FindAsync(variantId);
                if (variant == null) return false;

               
                var variants = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(
                    variant.Variants, _jsonOptions) ?? new List<Dictionary<string, string>>();

                if (valueIndex < 0 || valueIndex >= variants.Count)
                {
                    return false; // Invalid index
                }

                // Remove variant value
                variants.RemoveAt(valueIndex);
                variant.Variants = JsonSerializer.Serialize(variants, _jsonOptions);
                variant.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Dictionary<string, string>>> GetVariantValuesAsync(int variantId)
        {
            try
            {
                var variant = await _context.ProductVariants.FindAsync(variantId);
                if (variant == null) return new List<Dictionary<string, string>>();

                return JsonSerializer.Deserialize<List<Dictionary<string, string>>>(
                    variant.Variants, _jsonOptions) ?? new List<Dictionary<string, string>>();
            }
            catch
            {
                return new List<Dictionary<string, string>>();
            }
        }

        public async Task<ProductVariant?> GetProductVariantByIdAsync(int variantId)
        {
            return await _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.VariantId == variantId);
        }

        private bool AreDictionariesEqual(Dictionary<string, string> dict1, Dictionary<string, string> dict2)
        {
            if (dict1.Count != dict2.Count)
                return false;

            return dict1.Keys.All(key => dict2.ContainsKey(key));
        }

        public async Task<int> GetTotalProductsCountAsync(
            string? name = null,
            string? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            var query = _context.Products.Where(p => !p.IsDelete);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.ProductCategory.ProductCategoryTitle.Contains(category));

            if (minPrice.HasValue)
                query = query.Where(p => p.BasePrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.BasePrice <= maxPrice.Value);

            return await query.CountAsync();
        }
    }
}