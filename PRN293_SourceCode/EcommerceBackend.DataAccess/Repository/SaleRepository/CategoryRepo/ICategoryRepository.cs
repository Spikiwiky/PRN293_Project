using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository.SaleRepository.SaleCategory
{
    public interface ICategoryRepository
    {
        Task<ProductCategory> GetCategoryByIdAsync(int id);
        Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync();
        Task AddCategoryAsync(ProductCategory category);
        Task UpdateCategoryAsync(ProductCategory category);
        Task DeleteCategoryAsync(int id);
        Task SaveChangesAsync();
    }
}
