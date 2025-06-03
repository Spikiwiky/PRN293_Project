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

        // Predefined valid sizes
        private static readonly HashSet<string> ValidSizes = new(StringComparer.OrdinalIgnoreCase)
        {
            "XS", "S", "M", "L", "XL", "XXL"
        };

        // Predefined valid colors
        private static readonly HashSet<string> ValidColors = new(StringComparer.OrdinalIgnoreCase)
        {
            "Red", "Blue", "Green", "Black", "White", 
            "Yellow", "Purple", "Orange", "Pink", 
            "Brown", "Gray", "Navy"
        };

        public AdminProductController(
            IAdminProductService adminProductService,
            ILogger<AdminProductController> logger)
        {
            _adminProductService = adminProductService;
            _logger = logger;
        }

        private ActionResult ValidateProductVariant(AdminProductCreateDto createDto)
        {
            if (createDto.Variants?.Any() != true)
            {
                return BadRequest("At least one variant is required");
            }

            foreach (var variant in createDto.Variants)
            {
                if (string.IsNullOrEmpty(variant.Size) || string.IsNullOrEmpty(variant.Color))
                {
                    return BadRequest("Each variant must have both size and color");
                }

                if (!ValidSizes.Contains(variant.Size))
                {
                    return BadRequest($"Invalid size '{variant.Size}'. Valid sizes are: {string.Join(", ", ValidSizes)}");
                }

                if (!ValidColors.Contains(variant.Color))
                {
                    return BadRequest($"Invalid color '{variant.Color}'. Valid colors are: {string.Join(", ", ValidColors)}");
                }

                if (variant.Price <= 0)
                {
                    return BadRequest("Price must be greater than 0 for all variants");
                }
            }

            return Ok();
        }

        private ActionResult ValidateProductVariant(AdminProductUpdateDto updateDto)
        {
            if (updateDto.Variants?.Any() != true)
            {
                return BadRequest("At least one variant is required");
            }

            foreach (var variant in updateDto.Variants)
            {
                if (string.IsNullOrEmpty(variant.Size) || string.IsNullOrEmpty(variant.Color))
                {
                    return BadRequest("Each variant must have both size and color");
                }

                if (!ValidSizes.Contains(variant.Size))
                {
                    return BadRequest($"Invalid size '{variant.Size}'. Valid sizes are: {string.Join(", ", ValidSizes)}");
                }

                if (!ValidColors.Contains(variant.Color))
                {
                    return BadRequest($"Invalid color '{variant.Color}'. Valid colors are: {string.Join(", ", ValidColors)}");
                }

                if (variant.Price <= 0)
                {
                    return BadRequest("Price must be greater than 0 for all variants");
                }
            }

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<AdminProductDto>>> GetProducts(
            [FromQuery] string? name = null,
            [FromQuery] string? category = null,
            [FromQuery] string? size = null,
            [FromQuery] string? color = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] bool? isFeatured = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var products = await _adminProductService.SearchProductsAsync(
                    name, category, size, color, minPrice, maxPrice,
                    startDate, endDate, isFeatured, page, pageSize);

                var totalCount = await _adminProductService.GetTotalProductCountAsync();

                return Ok(new
                {
                    Products = products,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, "Internal server error");
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
                var validationResult = ValidateProductVariant(createDto);
                if (validationResult is BadRequestObjectResult)
                {
                    return validationResult;
                }

                if (string.IsNullOrEmpty(createDto.ProductName))
                {
                    return BadRequest("Product name is required");
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

                var validationResult = ValidateProductVariant(updateDto);
                if (validationResult is BadRequestObjectResult)
                {
                    return validationResult;
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
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] bool? isFeatured = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (!string.IsNullOrEmpty(size) && !ValidSizes.Contains(size))
                {
                    return BadRequest($"Invalid size. Valid sizes are: {string.Join(", ", ValidSizes)}");
                }

                if (!string.IsNullOrEmpty(color) && !ValidColors.Contains(color))
                {
                    return BadRequest($"Invalid color. Valid colors are: {string.Join(", ", ValidColors)}");
                }

                var products = await _adminProductService.SearchProductsAsync(
                    name, category, size, color, minPrice, maxPrice,
                    startDate, endDate, isFeatured, page, pageSize);
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

        [HttpGet("categories")]
        public async Task<ActionResult<List<BusinessObject.dtos.AdminDto.CategoryDto>>> GetCategories()
        {
            try
            {
                var categories = await _adminProductService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return StatusCode(500, "An error occurred while getting categories");
            }
        }
    }
} 