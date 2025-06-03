using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EcommerceBackend.BusinessObject.Services.ProductService;
using EcommerceBackend.BusinessObject.dtos.ProductDto;
using EcommerceBackend.BusinessObject.dtos.Shared;

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
        public async Task<ActionResult<List<ProductsDto>>> LoadProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var products = await _productService.LoadProductsAsync(page, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductsDto>> GetProduct(int id)
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
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string? name = null,
            [FromQuery] string? category = null,
            [FromQuery] string? size = null,
            [FromQuery] string? color = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation(
                    "Searching products with parameters: name={Name}, category={Category}, size={Size}, color={Color}, minPrice={MinPrice}, maxPrice={MaxPrice}, page={Page}, pageSize={PageSize}",
                    name, category, size, color, minPrice, maxPrice, page, pageSize);

                var products = await _productService.SearchProductsAsync(
                    name, category, size, color, minPrice, maxPrice, page, pageSize);

                _logger.LogInformation("Successfully found {Count} products", products.Count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                return StatusCode(500, "An error occurred while searching products");
            }
        }

        [HttpGet("{id}/variants")]
        public async Task<ActionResult<List<ProductVariant>>> GetProductVariants(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                return Ok(product.Variants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting variants for product {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the variants");
            }
        }

        [HttpPost("variants")]
        public async Task<ActionResult<ProductVariant>> AddVariant([FromBody] AddVariantDTO variant)
        {
            try
            {
                var result = await _productService.AddVariantAsync(variant);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding variant");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 