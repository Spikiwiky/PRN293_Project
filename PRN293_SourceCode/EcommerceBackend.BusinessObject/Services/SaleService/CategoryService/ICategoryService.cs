using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.SaleService.CategoryService
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(ProductCategory category);
        Task UpdateCategoryAsync(ProductCategory category);
        Task DeleteCategoryAsync(int id);
        Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync();
    }
}
