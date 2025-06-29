using EcommerceBackend.BusinessObject.DTOs;
using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.DataAccess.Repository;
using System.Text.Json;

namespace EcommerceBackend.BusinessObject.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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
            if (string.IsNullOrEmpty(variantDTO.Attributes) || variantDTO.Variants == null || !variantDTO.Variants.Any())
            {
                return false;
            }

            // Check if product exists
            var product = await _productRepository.GetProductByIdAsync(variantDTO.ProductId);
            if (product == null)
            {
                return false;
            }

            // Validate variant structure against product's available attributes
            var availableAttributes = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                product.AvailableAttributes, _jsonOptions);

            if (availableAttributes != null)
            {
                var variantAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(
                    variantDTO.Attributes, _jsonOptions);

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

            // Validate all variants have the same structure
            var firstVariant = variantDTO.Variants.First();
            foreach (var variantt in variantDTO.Variants.Skip(1))
            {
                if (!AreDictionariesEqual(firstVariant, variantt))
                {
                    return false;
                }
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
            // Validate input
            if (string.IsNullOrEmpty(variantDTO.Attributes) || variantDTO.Variants == null || !variantDTO.Variants.Any())
            {
                return false;
            }

            // Check if product exists
            var product = await _productRepository.GetProductByIdAsync(variantDTO.ProductId);
            if (product == null)
            {
                return false;
            }

            // Validate variant structure against product's available attributes
            var availableAttributes = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                product.AvailableAttributes, _jsonOptions);

            if (availableAttributes != null)
            {
                var variantAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(
                    variantDTO.Attributes, _jsonOptions);

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

            // Validate all variants have the same structure
            var firstVariant = variantDTO.Variants.First();
            foreach (var variantt in variantDTO.Variants.Skip(1))
            {
                if (!AreDictionariesEqual(firstVariant, variantt))
                {
                    return false;
                }
            }

            var variant = new ProductVariant
            {
                VariantId = variantDTO.VariantId,
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

            return await _productRepository.AddProductAttributeAsync(productId, attributeName, attributeValues);
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

            return await _productRepository.UpdateProductAttributeAsync(productId, attributeName, attributeValues);
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

            if (!AreDictionariesEqual(attributes, variantValue))
            {
                return false; // Structure doesn't match
            }

            return await _productRepository.AddVariantValueAsync(variantId, variantValue);
        }

        public async Task<bool> UpdateVariantValueAsync(int variantId, int valueIndex, Dictionary<string, string> variantValue)
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

            if (!AreDictionariesEqual(attributes, variantValue))
            {
                return false; // Structure doesn't match
            }

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

        public async Task<List<Dictionary<string, string>>> GetVariantValuesAsync(int variantId)
        {
            // Get variant to validate existence
            var productVariant = await _productRepository.GetProductVariantByIdAsync(variantId);
            if (productVariant == null)
            {
                return new List<Dictionary<string, string>>();
            }

            return await _productRepository.GetVariantValuesAsync(variantId);
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
                CategoryName = product.ProductCategory?.ProductCategoryTitle,
                Images = product.ProductImages?.Select(i => i.ImageUrl).ToList() ?? new List<string>(),
                Variants = variants,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        private List<Dictionary<string, string>> ParseVariantValues(string variantsJson)
        {
            var result = new List<Dictionary<string, string>>();
            if (string.IsNullOrEmpty(variantsJson)) return result;
            try
            {
                var list = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(variantsJson, _jsonOptions);
                if (list != null)
                {
                    foreach (var dict in list)
                    {
                        var newDict = new Dictionary<string, string>();
                        foreach (var kv in dict)
                        {
                            if (kv.Value.ValueKind == JsonValueKind.String)
                                newDict[kv.Key] = kv.Value.GetString();
                            else if (kv.Value.ValueKind == JsonValueKind.Number)
                                newDict[kv.Key] = kv.Value.GetRawText();
                            else
                                newDict[kv.Key] = kv.Value.ToString();
                        }
                        result.Add(newDict);
                    }
                }
            }
            catch { }
            return result;
        }

        private bool AreDictionariesEqual(Dictionary<string, string> dict1, Dictionary<string, string> dict2)
        {
            if (dict1.Count != dict2.Count)
                return false;

            return dict1.Keys.All(key => dict2.ContainsKey(key));
        }
    }
}