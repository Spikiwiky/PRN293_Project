using EcommerceBackend.BusinessObject.Services;
using EcommerceBackend.BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EcommerceBackend.API.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IProductService productService, ILogger<AdminController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // ===== PRODUCT MANAGEMENT =====
        
        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var products = await _productService.GetAllProductsAsync(page, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound("Product not found");
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO product)
        {
            try
            {
                var result = await _productService.CreateProductAsync(product);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO product)
        {
            try
            {
                var result = await _productService.UpdateProductAsync(id, product);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // ===== PRODUCT ATTRIBUTES MANAGEMENT =====

        [HttpGet("products/{productId}/attributes")]
        public async Task<IActionResult> GetProductAttributes(int productId)
        {
            try
            {
                var attributes = await _productService.GetProductAttributesAsync(productId);
                return Ok(attributes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product attributes for product {Id}", productId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("products/{productId}/attributes")]
        public async Task<IActionResult> AddProductAttribute(int productId, [FromBody] AddProductAttributeRequest request)
        {
            try
            {
                var result = await _productService.AddProductAttributeAsync(
                    productId, request.AttributeName, request.AttributeValues);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product attribute");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("products/{productId}/attributes/{attributeName}")]
        public async Task<IActionResult> UpdateProductAttribute(int productId, string attributeName, [FromBody] List<string> attributeValues)
        {
            try
            {
                var result = await _productService.UpdateProductAttributeAsync(
                    productId, attributeName, attributeValues);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product attribute");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("products/{productId}/attributes/{attributeName}")]
        public async Task<IActionResult> DeleteProductAttribute(int productId, string attributeName)
        {
            try
            {
                var result = await _productService.DeleteProductAttributeAsync(productId, attributeName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product attribute");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("products/{productId}/available-attributes")]
        public async Task<IActionResult> UpdateProductAvailableAttributes(int productId, string availableAttributes)
        {
            try
            {
                var result = await _productService.UpdateProductAttributesAsync(productId, availableAttributes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product available attributes");
                return StatusCode(500, "Internal server error");
            }
        }

        // ===== PRODUCT VARIANTS MANAGEMENT =====

        [HttpPost("variants")]
        public async Task<IActionResult> AddProductVariant([FromBody] ProductVariantDTO variant)
        {
            try
            {
                var result = await _productService.AddProductVariantAsync(variant);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product variant");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("variants")]
        public async Task<IActionResult> UpdateProductVariant([FromBody] ProductVariantDTO variant)
        {
            try
            {
                var result = await _productService.UpdateProductVariantAsync(variant);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product variant");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("variants/{variantId}")]
        public async Task<IActionResult> DeleteProductVariant(int variantId)
        {
            try
            {
                var result = await _productService.DeleteProductVariantAsync(variantId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product variant");
                return StatusCode(500, "Internal server error");
            }
        }

        // ===== VARIANT VALUES MANAGEMENT =====

        [HttpGet("variants/{variantId}/values")]
        public async Task<IActionResult> GetVariantValues(int variantId)
        {
            try
            {
                var values = await _productService.GetVariantValuesAsync(variantId);
                return Ok(values);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting variant values");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("variants/{variantId}/values")]
        public async Task<IActionResult> AddVariantValue(int variantId, [FromBody] Dictionary<string, string> variantValue)
        {
            try
            {
                var result = await _productService.AddVariantValueAsync(variantId, variantValue);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding variant value");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("variants/{variantId}/values/{valueIndex}")]
        public async Task<IActionResult> UpdateVariantValue(int variantId, int valueIndex, [FromBody] Dictionary<string, string> variantValue)
        {
            try
            {
                var result = await _productService.UpdateVariantValueAsync(variantId, valueIndex, variantValue);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating variant value");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("variants/{variantId}/values/{valueIndex}")]
        public async Task<IActionResult> DeleteVariantValue(int variantId, int valueIndex)
        {
            try
            {
                var result = await _productService.DeleteVariantValueAsync(variantId, valueIndex);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting variant value");
                return StatusCode(500, "Internal server error");
            }
        }

        // ===== SEARCH AND ANALYTICS =====

        [HttpGet("products/search")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string? name = null,
            [FromQuery] string? category = null,
            [FromQuery] Dictionary<string, string>? attributes = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var products = await _productService.SearchProductsAsync(
                    name, category, attributes, minPrice, maxPrice, page, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                return StatusCode(500, "An error occurred while searching products");
            }
        }

        [HttpGet("products/count")]
        public async Task<IActionResult> GetTotalProductsCount(
            [FromQuery] string? name = null,
            [FromQuery] string? category = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            try
            {
                var count = await _productService.GetTotalProductsCountAsync(
                    name, category, minPrice, maxPrice);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total products count");
                return StatusCode(500, "An error occurred while getting total products count");
            }
        }
    }

    public class AddProductAttributeRequest
    {
        public string AttributeName { get; set; } = string.Empty;
        public List<string> AttributeValues { get; set; } = new List<string>();
    }
}
