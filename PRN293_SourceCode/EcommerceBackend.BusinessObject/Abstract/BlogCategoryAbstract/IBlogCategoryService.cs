using EcommerceBackend.BusinessObject.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Abstract.BlogCategoryAbstract
{
    public interface IBlogCategoryService
    {
        Task<IEnumerable<BlogCategoryDto>> GetAllCategoriesAsync(bool includeDeleted = false);
        Task<BlogCategoryDto> GetCategoryByIdAsync(int id, bool includeDeleted = false);
        Task<BlogCategoryDto> CreateCategoryAsync(CreateBlogCategoryDto dto);
        Task UpdateCategoryAsync(UpdateBlogCategoryDto dto);
        Task DeleteCategoryAsync(int id, bool hardDelete = false);
        Task RestoreCategoryAsync(int id);
    }
}
