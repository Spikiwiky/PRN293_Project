using EcommerceBackend.DataAccess.Models;

namespace EcommerceBackend.DataAccess.Abstract
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllProductsWithDetailsAsync();
        Task<Product> GetProductWithDetailsAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<bool> SoftDeleteAsync(int id);
    }
} 