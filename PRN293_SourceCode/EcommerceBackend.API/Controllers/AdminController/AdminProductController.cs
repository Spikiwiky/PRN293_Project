using Microsoft.AspNetCore.Mvc;
using EcommerceBackend.BusinessObject.Services.AdminService;
using EcommerceBackend.BusinessObject.dtos.AdminDto;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceBackend.API.Controllers.AdminController
{
    [Route("api/admin/products")]
    [ApiController]
   
    public class AdminProductController : ControllerBase
    {
        private readonly IAdminProductService _adminProductService;
        private readonly ILogger<AdminProductController> _logger;

        public AdminProductController(
            IAdminProductService adminProductService,
            ILogger<AdminProductController> logger)
        {
            _adminProductService = adminProductService;
            _logger = logger;
        }

        private ActionResult ValidateProductVariant(string? size, string? color)
        {
            if ((string.IsNullOrEmpty(size) && !string.IsNullOrEmpty(color)) ||
                (!string.IsNullOrEmpty(size) && string.IsNullOrEmpty(color)))
            {
                return BadRequest("Both size and color must be provided together or both must be empty");
            }
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<AdminProductDto>>> GetProducts(
            [FromQuery] string? name = null,
            [FromQuery] string? category = null,
            [FromQuery] string? size = null,
            [FromQuery] string? color = null,
            [FromQuery] decimal? price = null,
            [FromQuery] bool? isFeatured = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation(
                    "Getting products with parameters: name={Name}, category={Category}, size={Size}, color={Color}, price={Price}, isFeatured={IsFeatured}, page={Page}, pageSize={PageSize}",
                    name, category, size, color, price, isFeatured, page, pageSize);

                var products = await _adminProductService.SearchProductsAsync(
                    name, category, size, color, null, price, isFeatured, page, pageSize);

                _logger.LogInformation("Successfully retrieved {Count} products", products.Count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products with parameters: name={Name}, category={Category}, size={Size}, color={Color}, price={Price}, isFeatured={IsFeatured}, page={Page}, pageSize={PageSize}",
                    name, category, size, color, price, isFeatured, page, pageSize);
                return StatusCode(500, new { message = "An error occurred while retrieving products", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdminProductDto>> GetProduct(int id)
        {
            try
            {
                var product = await _adminProductService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the product");
            }
        }

        [HttpPost]
        public async Task<ActionResult<AdminProductDto>> CreateProduct([FromBody] AdminProductCreateDto createDto)
        {
            try
            {
                
                var validationResult = ValidateProductVariant(createDto.Size, createDto.Color);
                if (validationResult is BadRequestObjectResult)
                {
                    return validationResult;
                }

              
                if (string.IsNullOrEmpty(createDto.ProductName))
                {
                    return BadRequest("Product name is required");
                }

                if (createDto.Price <= 0)
                {
                    return BadRequest("Price must be greater than 0");
                }

                var product = await _adminProductService.CreateProductAsync(createDto);
                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("variant"))
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "An error occurred while creating the product");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AdminProductDto>> UpdateProduct(
            int id,
            [FromBody] AdminProductUpdateDto updateDto)
        {
            try
            {
                if (id != updateDto.ProductId)
                {
                    return BadRequest("ID mismatch");
                }

             
                var validationResult = ValidateProductVariant(updateDto.Size, updateDto.Color);
                if (validationResult is BadRequestObjectResult)
                {
                    return validationResult;
                }

             
                if (updateDto.Price.HasValue && updateDto.Price <= 0)
                {
                    return BadRequest("Price must be greater than 0");
                }

                var product = await _adminProductService.UpdateProductAsync(updateDto);
                return Ok(product);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("variant"))
            {
                return Conflict(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the product");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _adminProductService.DeleteProductAsync(id);
                if (!result)
                {
                    return NotFound($"Product with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the product");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<AdminProductDto>>> SearchProducts(
            [FromQuery] string? name = null,
            [FromQuery] string? category = null,
            [FromQuery] string? size = null,
            [FromQuery] string? color = null,
            [FromQuery] string? variantId = null,
            [FromQuery] decimal? price = null,
            [FromQuery] bool? isFeatured = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate variant search parameters
                var validationResult = ValidateProductVariant(size, color);
                if (validationResult is BadRequestObjectResult)
                {
                    return validationResult;
                }

                var products = await _adminProductService.SearchProductsAsync(
                    name, category, size, color, variantId, price,
                    isFeatured, page, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                return StatusCode(500, "An error occurred while searching products");
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetTotalProductCount()
        {
            try
            {
                var count = await _adminProductService.GetTotalProductCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total product count");
                return StatusCode(500, "An error occurred while getting total product count");
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateProductStatus(int id, [FromBody] int status)
        {
            try
            {
                if (status < 0)
                {
                    return BadRequest("Status cannot be negative");
                }

                var result = await _adminProductService.UpdateProductStatusAsync(id, status);
                if (!result)
                {
                    return NotFound($"Product with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for product with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the product status");
            }
        }

        [HttpPatch("{id}/featured")]
        public async Task<IActionResult> UpdateProductFeatured(int id, [FromBody] bool isFeatured)
        {
            try
            {
                var result = await _adminProductService.UpdateProductFeaturedStatusAsync(id, isFeatured);
                if (!result)
                {
                    return NotFound($"Product with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating featured status for product with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the product featured status");
            }
        }
    }
} 