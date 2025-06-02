using EcommerceBackend.BusinessObject.dtos.ProductDto;
using EcommerceBackend.DataAccess;
using EcommerceBackend.DataAccess.Models;
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

        public ProductService(
            IProductRepository productRepository,
            EcommerceDBContext context,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _context = context;
            _logger = logger;
        }

        private ProductsDto MapToDto(Product product)
        {
            var dto = new ProductsDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductCategoryId = (int)product.ProductCategoryId,
                ProductCategoryTitle = product.ProductCategory?.ProductCategoryTitle,
                Description = product.Description,
                Status = product.Status ?? 1,
                IsDelete = product.IsDelete ?? false,
                ImageUrls = product.ProductImages?.Select(pi => pi.ImageUrl).ToList() ?? new List<string>()
            };

            if (!string.IsNullOrEmpty(product.Variants))
            {
                try
                {
                    var variants = JsonDocument.Parse(product.Variants).RootElement;
                    
                    if (variants.TryGetProperty("size", out var sizeElement))
                        dto.Size = sizeElement.GetString();
                    
                    if (variants.TryGetProperty("color", out var colorElement))
                        dto.Color = colorElement.GetString();
                    
                    if (variants.TryGetProperty("price", out var priceElement))
                        dto.Price = priceElement.GetDecimal();
                    
                   
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing variants JSON for product {ProductId}", product.ProductId);
                }
            }

            return dto;
        }

        public async Task<List<ProductsDto>> LoadProductsAsync(int page, int pageSize)
        {
            var products = await _productRepository.GetAllProductsAsync(page, pageSize);
            return products.Select(MapToDto).ToList();
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
                _logger.LogInformation(
                    "Searching products with parameters: name={Name}, category={Category}, size={Size}, color={Color}, minPrice={MinPrice}, maxPrice={MaxPrice}, page={Page}, pageSize={PageSize}",
                    name, category, size, color, minPrice, maxPrice, page, pageSize);

                var query = _context.Products
                    .Include(p => p.ProductCategory)
                    .Include(p => p.ProductImages)
                    .Where(p => (p.IsDelete == false || p.IsDelete == null) && p.Status == 1);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(p => p.ProductName.Contains(name));
                }

                if (!string.IsNullOrWhiteSpace(category))
                {
                    query = query.Where(p => p.ProductCategory.ProductCategoryTitle.Contains(category));
                }

                // Handle variants-based filtering
                if (!string.IsNullOrWhiteSpace(size) || !string.IsNullOrWhiteSpace(color) || minPrice.HasValue || maxPrice.HasValue)
                {
                    _logger.LogInformation("Performing variant-based filtering");
                    
                    var products = await query.ToListAsync();
                    
                    products = products.Where(p =>
                    {
                        if (string.IsNullOrEmpty(p.Variants)) return false;

                        try
                        {
                            var variants = JsonDocument.Parse(p.Variants).RootElement;

                            var matchesSize = string.IsNullOrWhiteSpace(size) || 
                                (variants.TryGetProperty("size", out var sizeElement) && 
                                 sizeElement.GetString()?.Contains(size, StringComparison.OrdinalIgnoreCase) == true);

                            var matchesColor = string.IsNullOrWhiteSpace(color) || 
                                (variants.TryGetProperty("color", out var colorElement) && 
                                 colorElement.GetString()?.Contains(color, StringComparison.OrdinalIgnoreCase) == true);

                            var matchesPrice = true;
                            if (variants.TryGetProperty("price", out var priceElement))
                            {
                                var price = priceElement.GetDecimal();
                                if (minPrice.HasValue)
                                    matchesPrice = matchesPrice && price >= minPrice.Value;
                                if (maxPrice.HasValue)
                                    matchesPrice = matchesPrice && price <= maxPrice.Value;
                            }

                            return matchesSize && matchesColor && matchesPrice;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error parsing variants JSON for product {ProductId}", p.ProductId);
                            return false;
                        }
                    }).ToList();

                    // Sort by createdAt in variants
                    products = products
                        .OrderByDescending(p => 
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(p.Variants)) return DateTime.MinValue;
                                var variants = JsonDocument.Parse(p.Variants).RootElement;
                                if (variants.TryGetProperty("createdAt", out var createdAtElement) &&
                                    DateTime.TryParse(createdAtElement.GetString(), out var createdAt))
                                {
                                    return createdAt;
                                }
                                return DateTime.MinValue;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Error parsing createdAt from variants for product {ProductId}", p.ProductId);
                                return DateTime.MinValue;
                            }
                        })
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    var dtos = products.Select(p => MapToDto(p)).ToList();
                    _logger.LogInformation("Successfully retrieved {Count} products after variant filtering", dtos.Count);
                    return dtos;
                }

                // If no variant filtering, use database pagination
                var totalCount = await query.CountAsync();
                _logger.LogInformation("Found {Count} total matching products before pagination", totalCount);

                var dbProducts = await query
                    .OrderByDescending(p => p.ProductId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = dbProducts.Select(p => MapToDto(p)).ToList();
                _logger.LogInformation("Successfully retrieved {Count} products after pagination", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                throw;
            }
        }
    }
}