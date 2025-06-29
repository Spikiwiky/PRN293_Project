using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EcommerceBackend.BusinessObject.Services;
using EcommerceBackend.BusinessObject.DTOs;

namespace EcommerceBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ProductDTO>>> SearchProducts(
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

        [HttpPost("{productId}/attributes")]
        public async Task<ActionResult<bool>> AddProductAttribute(
            int productId,
            [FromBody] AddProductAttributeRequest request)
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

        [HttpPut("{productId}/attributes/{attributeName}")]
        public async Task<ActionResult<bool>> UpdateProductAttribute(
            int productId,
            string attributeName,
            [FromBody] List<string> attributeValues)
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

        [HttpDelete("{productId}/attributes/{attributeName}")]
        public async Task<ActionResult<bool>> DeleteProductAttribute(
            int productId,
            string attributeName)
        {
            try
            {
                var result = await _productService.DeleteProductAttributeAsync(
                    productId, attributeName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product attribute");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{productId}/attributes")]
        public async Task<ActionResult<Dictionary<string, List<string>>>> GetProductAttributes(int productId)
        {
            try
            {
                var attributes = await _productService.GetProductAttributesAsync(productId);
                return Ok(attributes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product attributes");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{productId}/available-attributes")]
        public async Task<ActionResult<bool>> UpdateProductAttributes(
            int productId,
            [FromBody] Dictionary<string, List<string>> availableAttributes)
        {
            try
            {
                var jsonAttributes = System.Text.Json.JsonSerializer.Serialize(availableAttributes);
                var result = await _productService.UpdateProductAttributesAsync(productId, jsonAttributes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product available attributes");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("variants")]
        public async Task<ActionResult<bool>> AddProductVariant([FromBody] ProductVariantDTO variantDTO)
        {
            try
            {
                var result = await _productService.AddProductVariantAsync(variantDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product variant");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("variants")]
        public async Task<ActionResult<bool>> UpdateProductVariant([FromBody] ProductVariantDTO variantDTO)
        {
            try
            {
                var result = await _productService.UpdateProductVariantAsync(variantDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product variant");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("variants/{variantId}")]
        public async Task<ActionResult<bool>> DeleteProductVariant(int variantId)
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

        [HttpPost("variants/{variantId}/values")]
        public async Task<ActionResult<bool>> AddVariantValue(
            int variantId,
            [FromBody] Dictionary<string, string> variantValue)
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
        public async Task<ActionResult<bool>> UpdateVariantValue(
            int variantId,
            int valueIndex,
            [FromBody] Dictionary<string, string> variantValue)
        {
            try
            {
                var result = await _productService.UpdateVariantValueAsync(
                    variantId, valueIndex, variantValue);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating variant value");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("variants/{variantId}/values/{valueIndex}")]
        public async Task<ActionResult<bool>> DeleteVariantValue(
            int variantId,
            int valueIndex)
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

        [HttpGet("variants/{variantId}/values")]
        public async Task<ActionResult<List<Dictionary<string, string>>>> GetVariantValues(int variantId)
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

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetTotalProductsCount(
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