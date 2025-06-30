using EcommerceBackend.BusinessObject.DTOs;

namespace EcommerceBackend.BusinessObject.Services
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllProductsAsync(int page, int pageSize);
        Task<ProductDTO?> GetProductByIdAsync(int productId);
        Task<List<ProductDTO>> SearchProductsAsync(
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
        Task<bool> AddProductVariantAsync(ProductVariantDTO variantDTO);
        Task<bool> UpdateProductVariantAsync(ProductVariantDTO variantDTO);
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

        Task<bool> CreateProductAsync(ProductDTO product);
        Task<bool> UpdateProductAsync(int productId, ProductDTO product);
        Task<bool> DeleteProductAsync(int productId);
    }
} 