using EcommerceBackend.BusinessObject.dtos.AdminDto;
using EcommerceBackend.DataAccess;
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EcommerceBackend.BusinessObject.Services.AdminService
{
    public class AdminProductService : IAdminProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly EcommerceDBContext _context;
        private readonly ILogger<AdminProductService> _logger;

        public AdminProductService(
            IProductRepository productRepository,
            EcommerceDBContext context,
            ILogger<AdminProductService> logger)
        {
            _productRepository = productRepository;
            _context = context;
            _logger = logger;
        }

        private AdminProductDto MapToAdminDto(Product product)
        {
            var dto = new AdminProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductCategoryId = product.ProductCategoryId,
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
                    
                    if (variants.TryGetProperty("categories", out var categoriesElement))
                        dto.Category = categoriesElement.GetString();
                    
                    if (variants.TryGetProperty("variant_id", out var variantIdElement))
                        dto.VariantId = variantIdElement.GetString();
                    
                    if (variants.TryGetProperty("price", out var priceElement))
                        dto.Price = priceElement.GetDecimal();
                    
                    if (variants.TryGetProperty("stockQuantity", out var stockQuantityElement))
                        dto.StockQuantity = stockQuantityElement.GetInt32();
                    
                    if (variants.TryGetProperty("isFeatured", out var isFeaturedElement))
                        dto.IsFeatured = isFeaturedElement.GetBoolean();

                    // Parse timestamps
                    if (variants.TryGetProperty("createdAt", out var createdAtElement) && 
                        DateTime.TryParse(createdAtElement.GetString(), out var createdAt))
                        dto.CreatedAt = createdAt;

                    if (variants.TryGetProperty("updatedAt", out var updatedAtElement) && 
                        DateTime.TryParse(updatedAtElement.GetString(), out var updatedAt))
                        dto.UpdatedAt = updatedAt;

                    if (variants.TryGetProperty("createdBy", out var createdByElement))
                        dto.CreatedBy = createdByElement.GetString();

                    if (variants.TryGetProperty("updatedBy", out var updatedByElement))
                        dto.UpdatedBy = updatedByElement.GetString();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing variants JSON for product {ProductId}", product.ProductId);
                }
            }

            return dto;
        }

        public async Task<List<AdminProductDto>> GetAllProductsAsync(int page = 1, int pageSize = 10)
        {
            var products = await _productRepository.GetAllProductsAsync(page, pageSize);
            return products.Select(MapToAdminDto).ToList();
        }

        private async Task<Product?> GetProductWithDetailsAsync(int id)
        {
            return await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<AdminProductDto?> GetProductByIdAsync(int id)
        {
            var product = await GetProductWithDetailsAsync(id);
            return product == null ? null : MapToAdminDto(product);
        }

        private bool IsVariantDuplicateInProduct(string variants, string newSize, string newColor)
        {
            if (string.IsNullOrEmpty(variants)) return false;

            try
            {
                var variantDoc = JsonDocument.Parse(variants).RootElement;
                var existingSize = variantDoc.GetProperty("size").GetString();
                var existingColor = variantDoc.GetProperty("color").GetString();

                return string.Equals(existingSize, newSize, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(existingColor, newColor, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parsing variants JSON");
                return false;
            }
        }

        public async Task<AdminProductDto> CreateProductAsync(AdminProductCreateDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating new product with name: {ProductName}", createDto.ProductName);

                // Check for existing product with same name that is not deleted
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductName == createDto.ProductName && (!p.IsDelete.HasValue || !p.IsDelete.Value));

                if (existingProduct != null && !string.IsNullOrEmpty(createDto.Size) && !string.IsNullOrEmpty(createDto.Color))
                {
                    if (IsVariantDuplicateInProduct(existingProduct.Variants, createDto.Size, createDto.Color))
                    {
                        var error = $"Product already has a variant with Size={createDto.Size} and Color={createDto.Color}";
                        _logger.LogWarning(error);
                        throw new InvalidOperationException(error);
                    }
                }

                var now = DateTime.UtcNow;
                var variants = new
                {
                    size = createDto.Size,
                    color = createDto.Color,
                    categories = createDto.Category,
                    variant_id = createDto.VariantId ?? Guid.NewGuid().ToString(),
                    price = createDto.Price,
                    stockQuantity = createDto.StockQuantity,
                    isFeatured = createDto.IsFeatured,
                    createdAt = now.ToString("o"), // ISO 8601 format
                    updatedAt = now.ToString("o"),
                    createdBy = createDto.CreatedBy ?? "system",
                    updatedBy = createDto.CreatedBy ?? "system"
                };

                var product = new Product
                {
                    ProductName = createDto.ProductName,
                    ProductCategoryId = createDto.ProductCategoryId,
                    Description = createDto.Description,
                    Status = createDto.Status,
                    IsDelete = false,
                    Variants = JsonSerializer.Serialize(variants)
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                if (createDto.ImageUrls?.Any() == true)
                {
                    var productImages = createDto.ImageUrls.Select(url => new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = url
                    });
                    
                    _context.ProductImages.AddRange(productImages);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Successfully created product with ID: {ProductId}", product.ProductId);

                var result = await GetProductByIdAsync(product.ProductId);
                if (result == null)
                {
                    throw new Exception($"Failed to retrieve created product with ID {product.ProductId}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product with name: {ProductName}", createDto.ProductName);
                throw;
            }
        }

        public async Task<AdminProductDto> UpdateProductAsync(AdminProductUpdateDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", updateDto.ProductId);

                var product = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductId == updateDto.ProductId);

                if (product == null)
                {
                    var error = $"Product with ID {updateDto.ProductId} not found";
                    _logger.LogWarning(error);
                    throw new KeyNotFoundException(error);
                }

                // Check for duplicate variant in the same product
                if (!string.IsNullOrEmpty(updateDto.Size) && !string.IsNullOrEmpty(updateDto.Color))
                {
                    if (IsVariantDuplicateInProduct(product.Variants, updateDto.Size, updateDto.Color))
                    {
                        var error = $"Product already has a variant with Size={updateDto.Size} and Color={updateDto.Color}";
                        _logger.LogWarning(error);
                        throw new InvalidOperationException(error);
                    }
                }

                // Update basic properties
                if (!string.IsNullOrEmpty(updateDto.ProductName))
                    product.ProductName = updateDto.ProductName;
                
                if (updateDto.ProductCategoryId.HasValue)
                    product.ProductCategoryId = updateDto.ProductCategoryId;
                
                if (!string.IsNullOrEmpty(updateDto.Description))
                    product.Description = updateDto.Description;
                
                if (updateDto.Status.HasValue)
                    product.Status = updateDto.Status;

                // Update variants
                var now = DateTime.UtcNow;
                Dictionary<string, JsonElement> variantDict;

                if (!string.IsNullOrEmpty(product.Variants))
                {
                    var variants = JsonDocument.Parse(product.Variants).RootElement;
                    variantDict = variants.EnumerateObject()
                        .ToDictionary(p => p.Name, p => p.Value);

                    // Keep original createdAt and createdBy
                    if (!variantDict.ContainsKey("createdAt"))
                        variantDict["createdAt"] = JsonDocument.Parse($"\"{now:o}\"").RootElement;
                    if (!variantDict.ContainsKey("createdBy"))
                        variantDict["createdBy"] = JsonDocument.Parse("\"system\"").RootElement;
                }
                else
                {
                    variantDict = new Dictionary<string, JsonElement>();
                    variantDict["createdAt"] = JsonDocument.Parse($"\"{now:o}\"").RootElement;
                    variantDict["createdBy"] = JsonDocument.Parse("\"system\"").RootElement;
                }

                // Update variant properties
                if (!string.IsNullOrEmpty(updateDto.Size))
                    variantDict["size"] = JsonDocument.Parse($"\"{updateDto.Size}\"").RootElement;
                
                if (!string.IsNullOrEmpty(updateDto.Color))
                    variantDict["color"] = JsonDocument.Parse($"\"{updateDto.Color}\"").RootElement;
                
                if (!string.IsNullOrEmpty(updateDto.Category))
                    variantDict["categories"] = JsonDocument.Parse($"\"{updateDto.Category}\"").RootElement;
                
                if (!string.IsNullOrEmpty(updateDto.VariantId))
                    variantDict["variant_id"] = JsonDocument.Parse($"\"{updateDto.VariantId}\"").RootElement;
                
                if (updateDto.Price.HasValue)
                    variantDict["price"] = JsonDocument.Parse(updateDto.Price.ToString()!).RootElement;
                
                if (updateDto.StockQuantity.HasValue)
                    variantDict["stockQuantity"] = JsonDocument.Parse(updateDto.StockQuantity.ToString()!).RootElement;
                
                if (updateDto.IsFeatured.HasValue)
                    variantDict["isFeatured"] = JsonDocument.Parse(updateDto.IsFeatured.ToString()!.ToLower()).RootElement;

                // Update timestamps
                variantDict["updatedAt"] = JsonDocument.Parse($"\"{now:o}\"").RootElement;
                variantDict["updatedBy"] = JsonDocument.Parse($"\"{updateDto.UpdatedBy ?? "system"}\"").RootElement;

                product.Variants = JsonSerializer.Serialize(variantDict);

                // Update images if provided
                if (updateDto.ImageUrls != null)
                {
                    _logger.LogInformation("Updating images for product {ProductId}", updateDto.ProductId);
                    
                    // Remove existing images
                    _context.ProductImages.RemoveRange(product.ProductImages);

                    // Add new images
                    var productImages = updateDto.ImageUrls.Select(url => new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = url
                    });
                    
                    _context.ProductImages.AddRange(productImages);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated product {ProductId}", updateDto.ProductId);

                var result = await GetProductByIdAsync(product.ProductId);
                if (result == null)
                {
                    throw new Exception($"Failed to retrieve updated product with ID {product.ProductId}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", updateDto.ProductId);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                _logger.LogInformation("Starting deletion process for product ID {Id}", id);

             
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found for deletion", id);
                    return false;
                }

                _logger.LogInformation("Found product {ProductName} (ID: {Id}) for deletion", product.ProductName, id);

                // Set IsDelete flag
                product.IsDelete = true;
                
                // Mark entity as modified
                _context.Entry(product).State = EntityState.Modified;
                
                // Save changes
                var changes = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChanges returned: {changes} records affected", changes);

                if (changes > 0)
                {
                    _logger.LogInformation("Successfully marked product {ProductName} (ID: {Id}) as deleted", product.ProductName, id);
                    
                    // Verify the change
                    await _context.Entry(product).ReloadAsync();
                    var isDeleted = product.IsDelete == true;
                    _logger.LogInformation("Verification - Product IsDelete status: {IsDelete}", isDeleted);
                    
                    return isDeleted;
                }
                else
                {
                    _logger.LogWarning("No changes were saved to database for product ID {Id}", id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID {Id}", id);
                throw;
            }
        }

        public async Task<List<AdminProductDto>> SearchProductsAsync(
            string? name = null,
            string? category = null,
            string? size = null,
            string? color = null,
            string? variantId = null,
            decimal? price = null,
            bool? isFeatured = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                _logger.LogInformation(
                    "Searching products with parameters: name={Name}, category={Category}, size={Size}, color={Color}, variantId={VariantId}, price={Price}, isFeatured={IsFeatured}, page={Page}, pageSize={PageSize}",
                    name, category, size, color, variantId, price, isFeatured, page, pageSize);

                var query = _context.Products
                    .Include(p => p.ProductCategory)
                    .Include(p => p.ProductImages)
                    .Where(p => p.IsDelete == false || p.IsDelete == null);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(p => p.ProductName.Contains(name));
                }

                if (!string.IsNullOrWhiteSpace(category))
                {
                    query = query.Where(p => p.ProductCategory.ProductCategoryTitle.Contains(category));
                }

                // Handle variants-based filtering
                if (!string.IsNullOrWhiteSpace(size) || !string.IsNullOrWhiteSpace(color) || !string.IsNullOrWhiteSpace(variantId) || price.HasValue || isFeatured.HasValue)
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

                            var matchesVariantId = string.IsNullOrWhiteSpace(variantId) || 
                                (variants.TryGetProperty("variant_id", out var variantIdElement) && 
                                 variantIdElement.GetString() == variantId);

                            var matchesPrice = !price.HasValue || 
                                (variants.TryGetProperty("price", out var priceElement) && 
                                 priceElement.GetDecimal() == price.Value);

                            var matchesFeatured = !isFeatured.HasValue || 
                                (variants.TryGetProperty("isFeatured", out var featuredElement) && 
                                 featuredElement.GetBoolean() == isFeatured.Value);

                            return matchesSize && matchesColor && matchesVariantId && matchesPrice && matchesFeatured;
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

                    var dtos = products.Select(p => MapToAdminDto(p)).ToList();
                    _logger.LogInformation("Successfully retrieved {Count} products after variant filtering", dtos.Count);
                    return dtos;
                }

                // If no variant filtering, use database pagination
                var totalCount = await query.CountAsync();
                _logger.LogInformation("Found {Count} total matching products before pagination", totalCount);

                var dbProducts = await query
                    .OrderByDescending(p => p.ProductId) // Sort by ProductId as a fallback
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = dbProducts.Select(p => MapToAdminDto(p)).ToList();
                _logger.LogInformation("Successfully retrieved {Count} products after pagination", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                throw;
            }
        }

        public async Task<bool> UpdateProductStatusAsync(int id, int status)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductFeaturedStatusAsync(int id, bool isFeatured)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            if (string.IsNullOrEmpty(product.Variants))
            {
                product.Variants = JsonSerializer.Serialize(new { isFeatured });
            }
            else
            {
                var variants = JsonDocument.Parse(product.Variants).RootElement;
                var variantDict = new Dictionary<string, JsonElement>();
                foreach (var prop in variants.EnumerateObject())
                {
                    variantDict[prop.Name] = prop.Value;
                }
                variantDict["isFeatured"] = JsonDocument.Parse(isFeatured.ToString().ToLower()).RootElement;
                product.Variants = JsonSerializer.Serialize(variantDict);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalProductCountAsync()
        {
            return await _context.Products
                .Where(p => p.IsDelete == false || p.IsDelete == null)
                .CountAsync();
        }
    }
} 