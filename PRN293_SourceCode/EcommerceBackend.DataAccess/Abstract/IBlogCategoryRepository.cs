using EcommerceBackend.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Abstract
{
    public interface IBlogCategoryRepository
    {
        Task<IEnumerable<BlogCategory>> GetAllAsync(bool includeDeleted = false);
        Task<BlogCategory> GetByIdAsync(int id, bool includeDeleted = false);
        Task AddAsync(BlogCategory category);
        Task UpdateAsync(BlogCategory category);
        Task SoftDeleteAsync(int id);
        Task HardDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task<bool> ExistsAsync(int id, bool includeDeleted = false);
    }
}