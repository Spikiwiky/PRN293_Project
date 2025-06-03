using EcommerceBackend.BusinessObject.dtos.AdminDto;
using EcommerceBackend.BusinessObject.dtos.ProductDto;
using EcommerceBackend.BusinessObject.dtos.Shared;
using EcommerceBackend.DataAccess;
using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly EcommerceDBContext _context;
        private readonly ILogger<ProductService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductService(
            IProductRepository productRepository,
            EcommerceDBContext context,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _context = context;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        private List<ProductVariant> DeserializeVariants(string? variantsJson)
        {
            if (string.IsNullOrEmpty(variantsJson))
                return new List<ProductVariant>();

            try
            {
                var variants = JsonSerializer.Deserialize<List<ProductVariant>>(variantsJson, _jsonOptions);
                return variants ?? new List<ProductVariant>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing variants JSON");
                return new List<ProductVariant>();
            }
        }

        private string SerializeVariants(List<ProductVariant> variants)
        {
            try
            {
                return JsonSerializer.Serialize(variants, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serializing variants");
                return "[]";
            }
        }

        private async Task<ProductsDto> MapToDto(Product product)
        {
            return new ProductsDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description ?? string.Empty,
                ProductCategoryId = product.ProductCategoryId ?? 0,
                ProductCategoryTitle = product.ProductCategory?.ProductCategoryTitle ?? string.Empty,
                Status = product.Status ?? 1,
                IsDelete = product.IsDelete ?? false,
                ImageUrls = product.ProductImages?.Select(pi => pi.ImageUrl).ToList() ?? new List<string>(),
                Variants = DeserializeVariants(product.Variants)
            };
        }

        public async Task<List<ProductsDto>> GetAllProductsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var products = await _productRepository.GetAllProductsAsync(page, pageSize);
                var dtos = new List<ProductsDto>();
                foreach (var product in products)
                {
                    dtos.Add(await MapToDto(product));
                }
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                throw;
            }
        }

        public async Task<ProductsDto?> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.ProductCategory)
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => 
                        p.ProductId == id && 
                        p.IsDelete != true);

                return product == null ? null : await MapToDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by id {Id}", id);
                throw;
            }
        }

        public async Task<List<ProductsDto>> SearchProductsAsync(
            string? name = null,
            string? category = null,
            string? size = null,
            string? color = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.ProductCategory)
                    .Include(p => p.ProductImages)
                    .Where(p => p.IsDelete != true && p.Status == 1);

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(p => p.ProductName.Contains(name));

                if (!string.IsNullOrEmpty(category))
                    query = query.Where(p => p.ProductCategory.ProductCategoryTitle.Contains(category));

                var products = await query.ToListAsync();

                var filteredProducts = products.Where(p =>
                {
                    if (string.IsNullOrEmpty(size) && string.IsNullOrEmpty(color) &&
                        !minPrice.HasValue && !maxPrice.HasValue)
                    {
                        return true;
                    }

                    var variants = DeserializeVariants(p.Variants);
                    return variants.Any(v =>
                        (string.IsNullOrEmpty(size) || v.Size.Equals(size, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrEmpty(color) || v.Color.Equals(color, StringComparison.OrdinalIgnoreCase)) &&
                        (!minPrice.HasValue || v.Price >= minPrice.Value) &&
                        (!maxPrice.HasValue || v.Price <= maxPrice.Value));
                });

                var paginatedProducts = filteredProducts
                    .OrderByDescending(p => p.ProductId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var dtos = new List<ProductsDto>();
                foreach (var product in paginatedProducts)
                {
                    dtos.Add(await MapToDto(product));
                }
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                throw;
            }
        }

        public async Task<ProductsDto> UpdateProductAsync(ProductsDto productDto)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.ProductCategory)
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductId == productDto.ProductId);

                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {productDto.ProductId} not found");
                }

                product.Variants = SerializeVariants(productDto.Variants);
                await _context.SaveChangesAsync();
                return await MapToDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {Id}", productDto.ProductId);
                throw;
            }
        }

        public async Task<List<ProductsDto>> LoadProductsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.ProductCategory)
                    .Include(p => p.ProductImages)
                    .Where(p => p.IsDelete != true)
                    .Where(p => p.Status == 1)
                    .OrderByDescending(p => p.ProductId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = new List<ProductsDto>();
                foreach (var product in products)
                {
                    dtos.Add(await MapToDto(product));
                }
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                throw;
            }
        }

        public async Task<ProductVariant> AddVariantAsync(AddVariantDTO variant)
        {
            try
            {
                var product = await _context.Products.FindAsync(variant.ProductId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {variant.ProductId} not found");
                }

                var variants = DeserializeVariants(product.Variants);
                
                // Check for duplicate variant
                if (variants.Any(v => 
                    v.Size.Equals(variant.Size, StringComparison.OrdinalIgnoreCase) && 
                    v.Color.Equals(variant.Color, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"Variant with size {variant.Size} and color {variant.Color} already exists");
                }

                var newVariant = new ProductVariant
                {
                    VariantId = Guid.NewGuid().ToString(),
                    Size = variant.Size,
                    Color = variant.Color,
                    Price = variant.Price,
                    StockQuantity = variant.StockQuantity,
                    CreatedAt = DateTime.UtcNow
                };

                variants.Add(newVariant);
                product.Variants = SerializeVariants(variants);
                await _context.SaveChangesAsync();

                return newVariant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding variant to product {Id}", variant.ProductId);
                throw;
            }
        }

        public async Task<int> GetTotalProductCountAsync()
        {
            try
            {
                return await _context.Products
                    .Where(p => p.IsDelete != true)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total product count");
                throw;
            }
        }
    }
}