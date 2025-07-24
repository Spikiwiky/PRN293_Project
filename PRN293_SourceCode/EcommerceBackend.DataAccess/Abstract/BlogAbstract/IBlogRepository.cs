using EcommerceBackend.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Abstract.BlogAbstract
{
    public interface IBlogRepository
    {
        // Basic CRUD operations
        Task<IEnumerable<Blog>> GetAllAsync(bool includeDeleted = false);
        Task<Blog> GetByIdAsync(int id, bool includeDeleted = false);
        Task AddAsync(Blog blog);
        Task UpdateAsync(Blog blog);

        // Delete operations
        Task DeleteAsync(int id); // Legacy hard delete (consider deprecating)
        Task SoftDeleteAsync(int id);
        Task HardDeleteAsync(int id);
        Task RestoreAsync(int id);

        // Query operations
        Task<IEnumerable<Blog>> GetPagedAsync(int page, int pageSize, bool includeDeleted = false);
        Task<IEnumerable<Blog>> FindByTitleAsync(string title, bool includeDeleted = false);
        Task<IEnumerable<Blog>> GetByCategoryIdAsync(int categoryId, bool includeDeleted = false);

        // Validation operations
        Task<bool> ExistsAsync(int blogId);
        Task<bool> ExistsAsync(int blogId, bool includeDeleted = false);

        // Category operations
        Task<IEnumerable<BlogCategory>> GetAllCategoriesAsync();
        Task<Blog> CreateAsync(Blog blog, int categoryId); // Consider consolidating with AddAsync
    }
}