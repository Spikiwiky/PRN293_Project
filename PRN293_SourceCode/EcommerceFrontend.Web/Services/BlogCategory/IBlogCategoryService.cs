using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceFrontend.Web.Models.DTOs;

namespace EcommerceFrontend.Web.Services.Admin.BlogCategory
{
    public interface IBlogCategoryServiceWeb
    {
        Task<IEnumerable<BlogCategoryDto>> GetAllCategoriesAsync(bool includeDeleted = false);
        Task<BlogCategoryDto> GetCategoryByIdAsync(int id, bool includeDeleted = false);
        Task<BlogCategoryDto> CreateCategoryAsync(CreateBlogCategoryDto dto);
        Task UpdateCategoryAsync(UpdateBlogCategoryDto dto);
        Task DeleteCategoryAsync(int id, bool hardDelete = false);
        Task RestoreCategoryAsync(int id);
    }
}