using EcommerceBackend.API.Dtos;
using EcommerceBackend.BusinessObject.dtos.SaleDto;
using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.DataAccess.Repository.SaleRepository.ProductRepo;
using EcommerceBackend.DataAccess.Repository.SaleRepository.CategoryRepo;
using Microsoft.AspNetCore.Mvc;
using ProductImageDto = EcommerceBackend.API.Dtos.ProductImageDto;
using ProductVariantDto = EcommerceBackend.API.Dtos.ProductVariantDto;
using EcommerceBackend.BusinessObject.Services.SaleService.CategoryService;
using EcommerceBackend.BusinessObject.Services.SaleService.ProductService;

namespace EcommerceBackend.API.Controllers.SaleController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ISaleService _saleService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryService _categoryService;

        public SaleProductController(
            IProductRepository productRepository,
            ISaleService saleService,
            ICategoryRepository categoryRepository,
            ICategoryService categoryService)
        {
            _productRepository = productRepository;
            _saleService = saleService;
            _categoryRepository = categoryRepository;
            _categoryService = categoryService;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _saleService.GetAllProductsAsync();
            var responseDtos = products.Select(p => new EcommerceBackend.API.Dtos.ProductResponseDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                ProductCategoryId = p.ProductCategoryId,
                Brand = p.Brand,
                BasePrice = p.BasePrice,
                AvailableAttributes = p.AvailableAttributes,
                Status = p.Status,
                IsDelete = p.IsDelete,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            return Ok(responseDtos);
        }

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Dữ liệu sản phẩm không được để trống.");
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                ProductCategoryId = productDto.ProductCategoryId,
                Brand = productDto.Brand,
                BasePrice = productDto.BasePrice,
                AvailableAttributes = productDto.AvailableAttributes,
                Status = productDto.Status,
                IsDelete = productDto.IsDelete,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (product.ProductCategoryId.HasValue && product.ProductCategoryId.Value != 0)
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(product.ProductCategoryId.Value);
                if (category == null) return NotFound("Danh mục không tồn tại.");
                product.ProductCategory = category;
            }

            await _saleService.CreateProductAsync(product);

            if (productDto.ProductImages != null)
            {
                product.ProductImages = productDto.ProductImages.Select(img => new ProductImage
                {
                    ProductId = product.ProductId,
                    ImageUrl = img.ImageUrl
                }).ToList();
            }

            if (productDto.Variants != null)
            {
                product.Variants = productDto.Variants.Select(v => new ProductVariant
                {
                    ProductId = product.ProductId,
                    Attributes = productDto.AvailableAttributes,
                    Variants = v.Variants,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();
            }

            await _productRepository.SaveChangesAsync();

            var responseDto = new EcommerceBackend.BusinessObject.dtos.SaleDto.ProductResponseDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                ProductCategoryId = product.ProductCategoryId,
                Brand = product.Brand,
                BasePrice = product.BasePrice,
                AvailableAttributes = product.AvailableAttributes,
                Status = product.Status,
                IsDelete = product.IsDelete,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, responseDto);
        }


        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

            await _saleService.DeleteProductAsync(id);
            await _productRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var responseDtos = categories.Select(c => new ProductCategoryResponseDto
            {
                ProductCategoryId = c.ProductCategoryId,
                ProductCategoryTitle = c.ProductCategoryTitle,
                IsDelete = c?.IsDelete ?? false
            }).ToList();

            return Ok(responseDtos);
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Dữ liệu sản phẩm không được để trống.");
            }

            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

            // --- Update các trường cơ bản ---
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.ProductCategoryId = productDto.ProductCategoryId;
            product.Brand = productDto.Brand;
            product.BasePrice = productDto.BasePrice;
            product.AvailableAttributes = productDto.AvailableAttributes;
            product.Status = productDto.Status;
            product.IsDelete = productDto.IsDelete;
            product.UpdatedAt = DateTime.UtcNow;

            if (productDto.ProductImages != null)
            {
                var updatedImages = productDto.ProductImages.Select(img => new ProductImage
                {
                    ProductImageId = img.ProductImageId,
                    ImageUrl = img.ImageUrl,
                    ProductId = product.ProductId
                }).ToList();

                _productRepository.UpdateProductImages(product, updatedImages);
            }

            if (productDto.Variants != null)
            {
                var updatedVariants = productDto.Variants.Select(v => new ProductVariant
                {
                    VariantId = v.VariantId,
                    Attributes = v.Attributes,
                    Variants = v.Variants,
                    ProductId = product.ProductId
                }).ToList();

                _productRepository.UpdateProductVariants(product, updatedVariants);
            }

            await _productRepository.SaveChangesAsync();

            return Ok(new { Message = "Cập nhật sản phẩm thành công." });
        }


        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateProductCategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest("Dữ liệu danh mục không được để trống.");
            }

            var category = new ProductCategory
            {
                ProductCategoryTitle = categoryDto.ProductCategoryTitle,
                IsDelete = categoryDto.IsDelete
            };

            await _categoryService.CreateCategoryAsync(category);
            await _categoryRepository.SaveChangesAsync();

            var responseDto = new ProductCategoryResponseDto
            {
                ProductCategoryId = category.ProductCategoryId,
                ProductCategoryTitle = category.ProductCategoryTitle,
                IsDelete = category?.IsDelete ?? false
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.ProductCategoryId }, responseDto);
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateProductCategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest("Dữ liệu danh mục không được để trống.");
            }

            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound("Danh mục không tồn tại.");
            }

            category.ProductCategoryTitle = categoryDto.ProductCategoryTitle;
            category.IsDelete = categoryDto.IsDelete;

            await _categoryService.UpdateCategoryAsync(category);
            await _categoryRepository.SaveChangesAsync();

            var responseDto = new ProductCategoryResponseDto
            {
                ProductCategoryId = category.ProductCategoryId,
                ProductCategoryTitle = category.ProductCategoryTitle,
                IsDelete = category?.IsDelete ?? false
            };

            return Ok(responseDto);
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound("Danh mục không tồn tại.");
            }

            await _categoryService.DeleteCategoryAsync(id);
            await _categoryRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            var responseDto = new ProductDetailResponseDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                ProductCategoryId = product.ProductCategoryId,
                Brand = product.Brand,
                BasePrice = product.BasePrice,
                AvailableAttributes = product.AvailableAttributes,
                Status = product.Status,
                IsDelete = product.IsDelete,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                ProductImages = product.ProductImages?.Select(img => new ProductImageDto
                {
                    ImageUrl = img.ImageUrl
                }).ToList() ?? new List<ProductImageDto>(),

                Variants = product.Variants?.Select(v => new ProductVariantDto
                {
                    Attributes = v.Attributes,
                    Variants = v.Variants
                }).ToList() ?? new List<ProductVariantDto>()
            };

            return Ok(responseDto);
        }


        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            var responseDto = new ProductCategoryResponseDto
            {
                ProductCategoryId = category.ProductCategoryId,
                ProductCategoryTitle = category.ProductCategoryTitle,
                IsDelete = category?.IsDelete ?? false
            };
            return Ok(responseDto);
        }
    }
}