using EcommerceBackend.BusinessObject.DTOs;
using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.DataAccess.Repository;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace EcommerceBackend.BusinessObject.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<ProductDTO>> GetAllProductsAsync(int page, int pageSize)
        {
            var products = await _productRepository.GetAllProductsAsync(page, pageSize);
            return products.Select(MapToDTO).ToList();
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            return product != null ? MapToDTO(product) : null;
        }

        public async Task<List<ProductDTO>> SearchProductsAsync(
            string? name = null,
            string? category = null,
            Dictionary<string, string>? attributes = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 10)
        {
            var products = await _productRepository.SearchProductsAsync(
                name, category, attributes, minPrice, maxPrice, page, pageSize);
            return products.Select(MapToDTO).ToList();
        }

        public async Task<bool> UpdateProductAttributesAsync(int productId, string availableAttributes)
        {
            return await _productRepository.UpdateProductAttributesAsync(productId, availableAttributes);
        }

        public async Task<bool> AddProductVariantAsync(ProductVariantDTO variantDTO)
        {
            // Validate input
            if (variantDTO.Variants == null || !variantDTO.Variants.Any())
                return false;
            // Kiểm tra mỗi biến thể con đều có price và stock (nếu muốn)
            foreach (var v in variantDTO.Variants)
            {
                if (!v.ContainsKey("price") || !v.ContainsKey("stock"))
                    return false;
            }
            var variant = new ProductVariant
            {
                ProductId = variantDTO.ProductId,
                Attributes = variantDTO.Attributes,
                Variants = JsonSerializer.Serialize(variantDTO.Variants, _jsonOptions),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            return await _productRepository.AddProductVariantAsync(variant);
        }

        public async Task<bool> UpdateProductVariantAsync(ProductVariantDTO variantDTO)
        {
            if (variantDTO.VariantId == null)
                return false;
            if (variantDTO.Variants == null || !variantDTO.Variants.Any())
                return false;
            foreach (var v in variantDTO.Variants)
            {
                if (!v.ContainsKey("price") || !v.ContainsKey("stock"))
                    return false;
            }
            var variant = new ProductVariant
            {
                VariantId = variantDTO.VariantId.Value,
                ProductId = variantDTO.ProductId,
                Attributes = variantDTO.Attributes,
                Variants = JsonSerializer.Serialize(variantDTO.Variants, _jsonOptions),
                UpdatedAt = DateTime.UtcNow
            };
            return await _productRepository.UpdateProductVariantAsync(variant);
        }

        public async Task<bool> DeleteProductVariantAsync(int variantId)
        {
            return await _productRepository.DeleteProductVariantAsync(variantId);
        }

        public async Task<bool> AddProductAttributeAsync(int productId, string attributeName, List<string> attributeValues)
        {
            // Validate input
            if (string.IsNullOrEmpty(attributeName) || attributeValues == null || !attributeValues.Any())
            {
                return false;
            }

            // Check if product exists
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return false;
            }

            // Get current attributes
            var currentAttributes = await _productRepository.GetProductAttributesAsync(productId);
            if (currentAttributes.ContainsKey(attributeName))
            {
                return false; // Attribute already exists
            }

            var result = await _productRepository.AddProductAttributeAsync(productId, attributeName, attributeValues);

            // Đồng bộ lại attributes của các variant: copy y nguyên available_attributes
            var productAfter = await _productRepository.GetProductByIdAsync(productId);
            var availableAttrsJson = productAfter?.AvailableAttributes ?? "{}";
            var variants = await _productRepository.GetVariantsByProductIdAsync(productId);
            foreach (var variant in variants)
            {
                try
                {
                    variant.Attributes = availableAttrsJson;
                    await _productRepository.UpdateProductVariantAsync(variant, skipValidation: true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi đồng bộ attributes cho variantId={VariantId} của productId={ProductId}. Dữ liệu cũ: {OldAttributes}", variant.VariantId, productId, variant.Attributes);
                    throw;
                }
            }

            return result;
        }

        public async Task<bool> UpdateProductAttributeAsync(int productId, string attributeName, List<string> attributeValues)
        {
            // Validate input
            if (string.IsNullOrEmpty(attributeName) || attributeValues == null || !attributeValues.Any())
            {
                return false;
            }

            // Check if product exists
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return false;
            }

            // Get current attributes
            var currentAttributes = await _productRepository.GetProductAttributesAsync(productId);
            if (!currentAttributes.ContainsKey(attributeName))
            {
                return false; // Attribute doesn't exist
            }

            var result = await _productRepository.UpdateProductAttributeAsync(productId, attributeName, attributeValues);

            // Đồng bộ lại attributes của các variant: copy y nguyên available_attributes
            var productAfter = await _productRepository.GetProductByIdAsync(productId);
            var availableAttrsJson = productAfter?.AvailableAttributes ?? "{}";
            var variants = await _productRepository.GetVariantsByProductIdAsync(productId);
            foreach (var variant in variants)
            {
                try
                {
                    variant.Attributes = availableAttrsJson;
                    await _productRepository.UpdateProductVariantAsync(variant, skipValidation: true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi đồng bộ attributes cho variantId={VariantId} của productId={ProductId}. Dữ liệu cũ: {OldAttributes}", variant.VariantId, productId, variant.Attributes);
                    throw;
                }
            }

            return result;
        }

        public async Task<bool> DeleteProductAttributeAsync(int productId, string attributeName)
        {
            // Validate input
            if (string.IsNullOrEmpty(attributeName))
            {
                return false;
            }

            // Check if product exists
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return false;
            }

            // Get current attributes
            var currentAttributes = await _productRepository.GetProductAttributesAsync(productId);
            if (!currentAttributes.ContainsKey(attributeName))
            {
                return false; // Attribute doesn't exist
            }

            return await _productRepository.DeleteProductAttributeAsync(productId, attributeName);
        }

        public async Task<Dictionary<string, List<string>>> GetProductAttributesAsync(int productId)
        {
            // Check if product exists
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return new Dictionary<string, List<string>>();
            }

            return await _productRepository.GetProductAttributesAsync(productId);
        }

        public async Task<bool> AddVariantValueAsync(int variantId, Dictionary<string, string> variantValue)
        {
            // Validate input
            if (variantValue == null || !variantValue.Any())
            {
                return false;
            }

            // Get variant to validate structure
            var productVariant = await _productRepository.GetProductVariantByIdAsync(variantId);
            if (productVariant == null)
            {
                return false;
            }

            // Validate variant structure
            var attributes = JsonSerializer.Deserialize<Dictionary<string, string>>(
                productVariant.Attributes, _jsonOptions) ?? new Dictionary<string, string>();

            return await _productRepository.AddVariantValueAsync(variantId, variantValue);
        }

        public async Task<bool> UpdateVariantValueAsync(int variantId, int valueIndex, Dictionary<string, string> variantValue)
        {
            // Get variant to validate structure
            var productVariant = await _productRepository.GetProductVariantByIdAsync(variantId);
            if (productVariant == null)
            {
                return false;
            }

            // Validate variant structure
            var attributes = JsonSerializer.Deserialize<Dictionary<string, string>>(
                productVariant.Attributes, _jsonOptions) ?? new Dictionary<string, string>();

            return await _productRepository.UpdateVariantValueAsync(variantId, valueIndex, variantValue);
        }

        public async Task<bool> DeleteVariantValueAsync(int variantId, int valueIndex)
        {
            // Get variant to validate existence
            var productVariant = await _productRepository.GetProductVariantByIdAsync(variantId);
            if (productVariant == null)
            {
                return false;
            }

            return await _productRepository.DeleteVariantValueAsync(variantId, valueIndex);
        }

        public async Task<List<Dictionary<string, object>>> GetVariantValuesAsync(int variantId)
        {
            // Get variant to validate existence
            var productVariant = await _productRepository.GetProductVariantByIdAsync(variantId);
            if (productVariant == null)
            {
                return new List<Dictionary<string, object>>();
            }

            var result = await _productRepository.GetVariantValuesAsync(variantId);
            return ConvertToObjectList(result);
        }

        public async Task<int> GetTotalProductsCountAsync(
            string? name = null,
            string? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            return await _productRepository.GetTotalProductsCountAsync(
                name, category, minPrice, maxPrice);
        }

        public async Task<bool> CreateProductAsync(ProductDTO product)
        {
            // Map DTO to entity
            var entity = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                BasePrice = product.BasePrice,
                AvailableAttributes = product.AvailableAttributes,
                ProductCategoryId = product.CategoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDelete = false
            };
            return await _productRepository.CreateProductAsync(entity);
        }

        public async Task<bool> UpdateProductAsync(int productId, ProductDTO product)
        {
            try
            {
                // Nếu available_attributes null/rỗng, giữ nguyên giá trị cũ trong DB
                if (string.IsNullOrWhiteSpace(product.AvailableAttributes))
                {
                    var oldProduct = await _productRepository.GetProductByIdAsync(productId);
                    if (oldProduct != null && !string.IsNullOrWhiteSpace(oldProduct.AvailableAttributes))
                    {
                        product.AvailableAttributes = oldProduct.AvailableAttributes;
                    }
                    else
                    {
                        product.AvailableAttributes = "{}";
                    }
                }

                // Map DTO to entity
                var entity = new Product
                {
                    ProductId = productId,
                    Name = product.Name,
                    Description = product.Description,
                    Brand = product.Brand,
                    BasePrice = product.BasePrice,
                    AvailableAttributes = product.AvailableAttributes,
                    ProductCategoryId = product.CategoryId,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await _productRepository.UpdateProductAsync(entity);

                // Sau khi cập nhật thành công, nếu available_attributes thay đổi thì đồng bộ lại variant
                var oldProductCheck = await _productRepository.GetProductByIdAsync(productId);
                if (oldProductCheck != null && oldProductCheck.AvailableAttributes != product.AvailableAttributes)
                {
                    var availableAttrs = await _productRepository.GetProductAttributesAsync(productId);
                    var variants = await _productRepository.GetVariantsByProductIdAsync(productId);
                    foreach (var variant in variants)
                    {
                        try
                        {
                            var variantAttrs = string.IsNullOrWhiteSpace(variant.Attributes)
                                ? new Dictionary<string, List<string>>()
                                : JsonSerializer.Deserialize<Dictionary<string, List<string>>>(variant.Attributes, _jsonOptions) ?? new Dictionary<string, List<string>>();

                            foreach (var key in availableAttrs.Keys)
                            {
                                if (!variantAttrs.ContainsKey(key))
                                    variantAttrs[key] = new List<string>();
                            }
                            var keysToRemove = variantAttrs.Keys.Except(availableAttrs.Keys).ToList();
                            foreach (var key in keysToRemove)
                                variantAttrs.Remove(key);

                            variant.Attributes = JsonSerializer.Serialize(variantAttrs, _jsonOptions);
                            await _productRepository.UpdateProductVariantAsync(variant, skipValidation: true);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Lỗi khi đồng bộ attributes cho variantId={VariantId} của productId={ProductId}. Dữ liệu cũ: {OldAttributes}", variant.VariantId, productId, variant.Attributes);
                            throw;
                        }
                    }
                }

                if (!result)
                {
                    _logger.LogWarning("UpdateProductAsync failed for ProductId={ProductId}, CategoryId={CategoryId}", productId, product.CategoryId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UpdateProductAsync for ProductId={ProductId}, CategoryId={CategoryId}", productId, product.CategoryId);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            return await _productRepository.DeleteProductAsync(productId);
        }

        private ProductDTO MapToDTO(Product product)
        {
            var variants = product.Variants?.Select(v => new ProductVariantDTO
            {
                VariantId = v.VariantId,
                ProductId = v.ProductId,
                Attributes = v.Attributes,
                Variants = ParseVariantValues(v.Variants)
            }).ToList() ?? new List<ProductVariantDTO>();

            return new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                BasePrice = product.BasePrice,
                AvailableAttributes = product.AvailableAttributes,
                CategoryId = (int)product.ProductCategoryId,
                CategoryName = product.ProductCategory?.ProductCategoryTitle,
                Images = product.ProductImages?.Select(i => i.ImageUrl).ToList() ?? new List<string>(),
                Variants = variants,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        private List<Dictionary<string, object>> ParseVariantValues(string variantsJson)
        {
            var result = new List<Dictionary<string, object>>();
            if (string.IsNullOrEmpty(variantsJson)) return result;
            try
            {
                var list = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(variantsJson, _jsonOptions);
                if (list != null)
                {
                    foreach (var dict in list)
                    {
                        var objDict = dict.ToDictionary(kvp => kvp.Key, kvp => (object)JsonElementToObject(kvp.Value));
                        result.Add(objDict);
                    }
                }
            }
            catch { }
            return result;
        }

        private object JsonElementToObject(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Number:
                    if (element.TryGetInt32(out int i)) return i;
                    if (element.TryGetDecimal(out decimal d)) return d;
                    return element.GetDouble();
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();
                default:
                    return element.ToString();
            }
        }

        private List<Dictionary<string, object>> ConvertToObjectList(List<Dictionary<string, string>> list)
        {
            return list.Select(dict => dict.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value)).ToList();
        }

        private Dictionary<string, object> ConvertToObjectDict(Dictionary<string, string> dict)
        {
            return dict.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);
        }
    }
}