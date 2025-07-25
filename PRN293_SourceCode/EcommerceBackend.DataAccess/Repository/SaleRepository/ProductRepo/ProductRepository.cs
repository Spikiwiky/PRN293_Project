using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository.SaleRepository.ProductRepo
{
    public class ProductRepository : IProductRepository
    {
        readonly EcommerceDBContext _context;
        public ProductRepository(EcommerceDBContext context)
        {
            _context = context;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImages)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDelete && p.Status == 1);
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImages)
                .Include(p => p.Variants)
                .ToListAsync();
        }
        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _context.SaveChangesAsync();
        }


        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDelete = true;
                product.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
        public void UpdateProductImages(Product product, List<ProductImage> updatedImages)
        {
            _context.Entry(product).Collection(p => p.ProductImages).Load();

            var removedImages = product.ProductImages
                .Where(pi => !updatedImages.Any(ui => ui.ProductImageId == pi.ProductImageId))
                .ToList();
            _context.ProductImages.RemoveRange(removedImages);

            foreach (var updatedImage in updatedImages)
            {
                if (updatedImage.ProductImageId > 0)
                {
                    var existingImage = _context.ProductImages
                        .FirstOrDefault(i => i.ProductImageId == updatedImage.ProductImageId);

                    if (existingImage != null)
                    {
                        existingImage.ImageUrl = updatedImage.ImageUrl; 
                    }
                }
                else
                {
                    updatedImage.ProductId = product.ProductId;
                    _context.ProductImages.Add(updatedImage);
                }
            }
        }

        public void UpdateProductVariants(Product product, List<ProductVariant> updatedVariants)
        {
            _context.Entry(product).Collection(p => p.Variants).Load();

            var removedVariants = product.Variants
                .Where(v => !updatedVariants.Any(uv => uv.VariantId == v.VariantId))
                .ToList();
            _context.ProductVariants.RemoveRange(removedVariants);

            foreach (var updatedVariant in updatedVariants)
            {
                var existingVariant = product.Variants
                    .FirstOrDefault(v => v.VariantId == updatedVariant.VariantId);

                if (existingVariant != null)
                {
                    existingVariant.Attributes = updatedVariant.Attributes;
                    existingVariant.Variants = updatedVariant.Variants;
                    existingVariant.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    updatedVariant.ProductId = product.ProductId;
                    updatedVariant.CreatedAt = DateTime.UtcNow;
                    updatedVariant.UpdatedAt = DateTime.UtcNow;
                    _context.ProductVariants.Add(updatedVariant);
                }
            }
        }
        public async Task<ProductVariant> GetProductVariantAsync(int productId, string variantId)
        {
            return await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.ProductId == productId && v.VariantId.ToString() == variantId);
        }
    }
}
