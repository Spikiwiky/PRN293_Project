using EcommerceBackend.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // **Mới thêm**
        Task<IEnumerable<ProductVariant>> GetProductVariantsByProductIdAsync(int productId);
    }
}
