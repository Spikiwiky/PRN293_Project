using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceBackend.DataAccess.Models;
namespace EcommerceBackend.DataAccess.Repository.SaleRepository.ProductRepo
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task SaveChangesAsync();
        void UpdateProductImages(Product product, List<ProductImage> images);
        void UpdateProductVariants(Product product, List<ProductVariant> variants);
        Task<ProductVariant> GetProductVariantAsync(int productId, string variantId);
    }
}
