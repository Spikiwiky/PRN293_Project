//using EcommerceBackend.BusinessObject.dtos.Shared;
//using EcommerceBackend.DataAccess.Models;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//using EcommerceBackend.DataAccess;
//using EcommerceBackend.DataAccess.Repository;
//using Microsoft.EntityFrameworkCore;
//using EcommerceBackend.BusinessObject.dtos.SaleDto;

//namespace EcommerceBackend.BusinessObject.Services.SaleService
//{
//    public class SaleProductService : ISaleProductService
//    {
//        private readonly IProductRepository _productRepository;
//        private readonly EcommerceDBContext _context;
//        private readonly ILogger<SaleProductService> _logger;
//        private readonly JsonSerializerOptions _jsonOptions;

//        private static readonly string[] ValidSizes = { "XS", "S", "M", "L", "XL", "XXL" };
//        private static readonly string[] ValidColors = { "Red", "Blue", "Green", "Black", "White", "Yellow", "Purple", "Orange", "Pink", "Brown", "Gray", "Navy" };

//        public SaleProductService(
//            IProductRepository productRepository,
//            EcommerceDBContext context,
//            ILogger<SaleProductService> logger)
//        {
//            _productRepository = productRepository;
//            _context = context;
//            _logger = logger;
//            _jsonOptions = new JsonSerializerOptions
//            {
//                PropertyNameCaseInsensitive = true,
//                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//            };
//        }

//        #region Helper Methods

//        private async Task<SaleProductDto> MapToDto(Product product)
//        {
//            var variants = DeserializeVariants(product.Variants);
//            var dto = new SaleProductDto
//            {
//                ProductId = product.ProductId,
//                ProductName = product.ProductName ?? string.Empty,
//                Description = product.Description ?? string.Empty,
//                ProductCategoryId = product.ProductCategoryId ?? 0,
//                ProductCategoryTitle = product.ProductCategory?.ProductCategoryTitle ?? string.Empty,
//                Status = product.Status ?? 1,
//                IsDelete = product.IsDelete ?? false,
//                ImageUrls = product.ProductImages?.Select(pi => pi.ImageUrl).ToList() ?? new List<string>(),
//                Variants = variants
//            };

//            return dto;
//        }

//        private List<ProductVariant> DeserializeVariants(string? variantsJson)
//        {
//            if (string.IsNullOrEmpty(variantsJson))
//                return new List<ProductVariant>();

//            try
//            {
//                var variants = JsonSerializer.Deserialize<List<ProductVariant>>(variantsJson, _jsonOptions);
//                return variants ?? new List<ProductVariant>();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deserializing variants JSON");
//                return new List<ProductVariant>();
//            }
//        }

//        private string SerializeVariants(List<ProductVariant> variants)
//        {
//            try
//            {
//                return JsonSerializer.Serialize(variants, _jsonOptions);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error serializing variants");
//                return "[]";
//            }
//        }

//        private bool IsValidVariant(ProductVariant variant)
//        {
//            if (variant == null) return false;

//            return !string.IsNullOrEmpty(variant.Size) &&
//                   !string.IsNullOrEmpty(variant.Color) &&
//                   variant.Price > 0 &&
//                   variant.StockQuantity >= 0 &&
//                   ValidSizes.Contains(variant.Size, StringComparer.OrdinalIgnoreCase) &&
//                   ValidColors.Contains(variant.Color, StringComparer.OrdinalIgnoreCase);
//        }

//        private bool IsVariantDuplicate(List<ProductVariant> variants, ProductVariant newVariant)
//        {
//            if (newVariant == null) return false;

//            return variants.Any(v =>
//                string.Equals(v.Size, newVariant.Size, StringComparison.OrdinalIgnoreCase) &&
//                string.Equals(v.Color, newVariant.Color, StringComparison.OrdinalIgnoreCase) &&
//                v.VariantId != newVariant.VariantId);
//        }

//        #endregion

//        #region Public Methods

//        public async Task<List<SaleProductDto>> GetAllProductsAsync(int page = 1, int pageSize = 10)
//        {
//            try
//            {
//                var products = await _context.Products
//                    .Include(p => p.ProductCategory)
//                    .Include(p => p.ProductImages)
//                    .Where(p => p.IsDelete != true)
//                    .OrderByDescending(p => p.ProductId)
//                    .Skip((page - 1) * pageSize)
//                    .Take(pageSize)
//                    .ToListAsync();

//                var tasks = products.Select(MapToDto);
//                var dtos = await Task.WhenAll(tasks);
//                return dtos.ToList();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting all products");
//                throw;
//            }
//        }

//        public async Task<SaleProductDto?> GetProductByIdAsync(int id)
//        {
//            try
//            {
//                var product = await _context.Products
//                    .Include(p => p.ProductCategory)
//                    .Include(p => p.ProductImages)
//                    .FirstOrDefaultAsync(p => p.ProductId == id && p.IsDelete != true);

//                return product == null ? null : await MapToDto(product);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting product by id {Id}", id);
//                throw;
//            }
//        }

//        public async Task<SaleProductDto> CreateProductAsync(SaleProductCreateDto createDto)
//        {
//            try
//            {
//                var product = new Product
//                {
//                    ProductName = createDto.ProductName,
//                    Description = createDto.Description,
//                    ProductCategoryId = createDto.ProductCategoryId,
//                    Status = createDto.Status,
//                    IsDelete = false
//                };

//                // Handle variants
//                var variants = new List<ProductVariant>();
//                if (createDto.Variants?.Any() == true)
//                {
//                    foreach (var variant in createDto.Variants)
//                    {
//                        if (!IsValidVariant(variant))
//                        {
//                            throw new ArgumentException($"Invalid variant: Size={variant.Size}, Color={variant.Color}");
//                        }

//                        variant.VariantId = $"{variant.Size}-{variant.Color}";
//                        variants.Add(variant);
//                    }
//                }

//                product.Variants = SerializeVariants(variants);

//                // First save the product to get its ID
//                _context.Products.Add(product);
//                await _context.SaveChangesAsync();

//                // Now handle image URLs with the product ID
//                if (createDto.ImageUrls?.Any() == true)
//                {
//                    var productImages = createDto.ImageUrls
//                        .Where(url => !string.IsNullOrWhiteSpace(url))  // Filter out null or empty URLs
//                        .Select(url => new ProductImage
//                        {
//                            ImageUrl = url.Trim(),  // Trim any whitespace
//                            ProductId = product.ProductId
//                        })
//                        .ToList();

//                    if (productImages.Any())
//                    {
//                        _context.ProductImages.AddRange(productImages);
//                        await _context.SaveChangesAsync();
//                    }
//                    else
//                    {
//                        // If no valid images provided, add a default image
//                        var defaultImage = new ProductImage
//                        {
//                            ImageUrl = "https://via.placeholder.com/300",  // Default placeholder image
//                            ProductId = product.ProductId
//                        };
//                        _context.ProductImages.Add(defaultImage);
//                        await _context.SaveChangesAsync();
//                    }
//                }
//                else
//                {
//                    // If no images provided, add a default image
//                    var defaultImage = new ProductImage
//                    {
//                        ImageUrl = "https://via.placeholder.com/300",  // Default placeholder image
//                        ProductId = product.ProductId
//                    };
//                    _context.ProductImages.Add(defaultImage);
//                    await _context.SaveChangesAsync();
//                }

//                var dto = await MapToDto(product);
//                return dto;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating product");
//                throw;
//            }
//        }

//        public async Task<SaleProductDto> UpdateProductAsync(SaleProductUpdateDto updateDto)
//        {
//            try
//            {
//                var product = await _context.Products
//                    .Include(p => p.ProductCategory)
//                    .Include(p => p.ProductImages)
//                    .FirstOrDefaultAsync(p => p.ProductId == updateDto.ProductId);

//                if (product == null)
//                {
//                    throw new KeyNotFoundException($"Product with ID {updateDto.ProductId} not found");
//                }

//                // Update basic properties
//                if (!string.IsNullOrEmpty(updateDto.ProductName))
//                    product.ProductName = updateDto.ProductName;

//                if (updateDto.Description != null)
//                    product.Description = updateDto.Description;

//                if (updateDto.ProductCategoryId.HasValue)
//                    product.ProductCategoryId = updateDto.ProductCategoryId;

//                if (updateDto.Status.HasValue)
//                    product.Status = updateDto.Status;

//                // Handle variants
//                if (updateDto.Variants?.Any() == true)
//                {
//                    var existingVariants = DeserializeVariants(product.Variants);

//                    foreach (var variant in updateDto.Variants)
//                    {
//                        if (!IsValidVariant(variant))
//                        {
//                            throw new ArgumentException($"Invalid variant: Size={variant.Size}, Color={variant.Color}");
//                        }

//                        var existingVariant = existingVariants.FirstOrDefault(v => v.VariantId == $"{variant.Size}-{variant.Color}");
//                        if (existingVariant != null)
//                        {
//                            // Update existing variant
//                            existingVariant.Size = variant.Size;
//                            existingVariant.Color = variant.Color;
//                            existingVariant.Categories = variant.Categories;
//                            existingVariant.Price = variant.Price;
//                            existingVariant.StockQuantity = variant.StockQuantity;
//                            existingVariant.IsFeatured = variant.IsFeatured;
//                        }
//                        else
//                        {
//                            // Add new variant
//                            if (IsVariantDuplicate(existingVariants, variant))
//                            {
//                                throw new ArgumentException($"Duplicate variant: Size={variant.Size}, Color={variant.Color}");
//                            }

//                            variant.VariantId = $"{variant.Size}-{variant.Color}";
//                            existingVariants.Add(variant);
//                        }
//                    }

//                    product.Variants = SerializeVariants(existingVariants);
//                }
//                // Handle legacy fields
//                else if (!string.IsNullOrEmpty(updateDto.Size) && !string.IsNullOrEmpty(updateDto.Color))
//                {
//                    var variant = new ProductVariant
//                    {
//                        Size = updateDto.Size,
//                        Color = updateDto.Color,
//                        Categories = updateDto.Category ?? string.Empty,
//                        VariantId = $"{updateDto.Size}-{updateDto.Color}",
//                        Price = updateDto.Price ?? 0,
//                        StockQuantity = updateDto.StockQuantity ?? 0,
//                        IsFeatured = updateDto.IsFeatured ?? false
//                    };

//                    if (!IsValidVariant(variant))
//                    {
//                        throw new ArgumentException($"Invalid variant: Size={variant.Size}, Color={variant.Color}");
//                    }

//                    var variants = new List<ProductVariant> { variant };
//                    product.Variants = SerializeVariants(variants);
//                }

//                // Handle image URLs
//                if (updateDto.ImageUrls != null)
//                {
//                    product.ProductImages?.Clear();
//                    product.ProductImages = updateDto.ImageUrls
//                        .Select(url => new ProductImage { ImageUrl = url })
//                        .ToList();
//                }

//                await _context.SaveChangesAsync();

//                var dto = await MapToDto(product);
//                dto.UpdatedAt = DateTime.UtcNow;
//                dto.UpdatedBy = updateDto.UpdatedBy;
//                return dto;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating product {Id}", updateDto.ProductId);
//                throw;
//            }
//        }

//        public async Task<List<SaleProductDto>> SearchProductsAsync(
//            string? name = null,
//            string? category = null,
//            string? size = null,
//            string? color = null,
//            decimal? minPrice = null,
//            decimal? maxPrice = null,
//            DateTime? startDate = null,
//            DateTime? endDate = null,
//            bool? isFeatured = null,
//            int page = 1,
//            int pageSize = 10)
//        {
//            try
//            {
//                var query = _context.Products
//                    .Include(p => p.ProductCategory)
//                    .Include(p => p.ProductImages)
//                    .Where(p => p.IsDelete != true);

//                if (!string.IsNullOrEmpty(name))
//                {
//                    query = query.Where(p => p.ProductName.Contains(name));
//                }

//                if (!string.IsNullOrEmpty(category))
//                {
//                    query = query.Where(p => p.ProductCategory.ProductCategoryTitle.Contains(category));
//                }

//                var products = await query
//                    .OrderByDescending(p => p.ProductId)
//                    .Skip((page - 1) * pageSize)
//                    .Take(pageSize)
//                    .ToListAsync();

//                var tasks = products.Select(async product => {
//                    var dto = await MapToDto(product);

//                    // Apply variant filters
//                    if (!string.IsNullOrEmpty(size) || !string.IsNullOrEmpty(color) ||
//                        minPrice.HasValue || maxPrice.HasValue || isFeatured.HasValue)
//                    {
//                        // Filter variants
//                        dto.Variants = dto.Variants.Where(v =>
//                            (string.IsNullOrEmpty(size) || v.Size.Equals(size, StringComparison.OrdinalIgnoreCase)) &&
//                            (string.IsNullOrEmpty(color) || v.Color.Equals(color, StringComparison.OrdinalIgnoreCase)) &&
//                            (!minPrice.HasValue || v.Price >= minPrice.Value) &&
//                            (!maxPrice.HasValue || v.Price <= maxPrice.Value) &&
//                            (!isFeatured.HasValue || v.IsFeatured == isFeatured.Value)
//                        ).ToList();

//                        // Only return products that have matching variants
//                        if (!dto.Variants.Any())
//                        {
//                            return null;
//                        }
//                    }
//                    return dto;
//                });

//                var results = await Task.WhenAll(tasks);
//                return results.Where(dto => dto != null).ToList();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error searching products");
//                throw;
//            }
//        }

//        public async Task<bool> DeleteProductAsync(int id)
//        {
//            try
//            {
//                var product = await _context.Products.FindAsync(id);
//                if (product == null) return false;

//                product.IsDelete = true;
//                await _context.SaveChangesAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting product {Id}", id);
//                throw;
//            }
//        }

//        public async Task<bool> UpdateProductStatusAsync(int id, int status)
//        {
//            try
//            {
//                var product = await _context.Products.FindAsync(id);
//                if (product == null) return false;

//                product.Status = status;
//                await _context.SaveChangesAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating product status {Id}", id);
//                throw;
//            }
//        }

//        public async Task<bool> UpdateProductFeaturedStatusAsync(int id, bool isFeatured)
//        {
//            try
//            {
//                var product = await _context.Products.FindAsync(id);
//                if (product == null) return false;

//                var variants = DeserializeVariants(product.Variants);
//                foreach (var variant in variants)
//                {
//                    variant.IsFeatured = isFeatured;
//                }

//                product.Variants = SerializeVariants(variants);
//                await _context.SaveChangesAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating product featured status {Id}", id);
//                throw;
//            }
//        }

//        public async Task<int> GetTotalProductCountAsync()
//        {
//            try
//            {
//                return await _context.Products
//                    .Where(p => p.IsDelete != true)
//                    .CountAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting total product count");
//                throw;
//            }
//        }

//        public async Task<List<CategoryDto>> GetCategoriesAsync()
//        {
//            try
//            {
//                return await _context.ProductCategories
//                    .Where(c => c.IsDelete != true)
//                    .Select(c => new CategoryDto
//                    {
//                        Id = c.ProductCategoryId,
//                        Name = c.ProductCategoryTitle ?? string.Empty
//                    })
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting categories");
//                throw;
//            }
//        }

//        #endregion
//    }
//}

