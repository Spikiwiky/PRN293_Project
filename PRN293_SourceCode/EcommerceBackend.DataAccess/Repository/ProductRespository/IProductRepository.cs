using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceBackend.DataAccess.Models;

namespace EcommerceBackend.DataAccess.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync(int page, int pageSize);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<ProductVariant?> GetProductVariantByIdAsync(int variantId);
        Task<List<Product>> SearchProductsAsync(
            string? name = null,
            string? category = null,
            Dictionary<string, string>? attributes = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 10);
        Task<int> GetTotalProductsCountAsync(
            string? name = null,
            string? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null);
        Task<bool> UpdateProductAttributesAsync(int productId, string availableAttributes);
        Task<bool> AddProductVariantAsync(ProductVariant variant);
        Task<bool> UpdateProductVariantAsync(ProductVariant variant);
        Task<bool> UpdateProductVariantAsync(ProductVariant variant, bool skipValidation = false);
        Task<bool> DeleteProductVariantAsync(int variantId);

        // New methods for managing product attributes
        Task<bool> AddProductAttributeAsync(int productId, string attributeName, List<string> attributeValues);
        Task<bool> UpdateProductAttributeAsync(int productId, string attributeName, List<string> attributeValues);
        Task<bool> DeleteProductAttributeAsync(int productId, string attributeName);
        Task<Dictionary<string, List<string>>> GetProductAttributesAsync(int productId);

        // New methods for managing variant values
        Task<bool> AddVariantValueAsync(int variantId, Dictionary<string, string> variantValue);
        Task<bool> UpdateVariantValueAsync(int variantId, int valueIndex, Dictionary<string, string> variantValue);
        Task<bool> DeleteVariantValueAsync(int variantId, int valueIndex);
        Task<List<Dictionary<string, string>>> GetVariantValuesAsync(int variantId);

        Task<bool> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int productId);
        Task<bool> AddProductImageAsync(int productId, string imageUrl);
        Task<List<ProductVariant>> GetVariantsByProductIdAsync(int productId);
    }
}
