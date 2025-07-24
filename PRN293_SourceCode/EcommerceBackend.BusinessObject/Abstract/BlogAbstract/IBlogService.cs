using EcommerceBackend.BusinessObject.dtos;

using EcommerceBackend.BusinessObject.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Abstract.BlogAbstract
{
    public interface IBlogService
    {
        Task<IEnumerable<BlogDto>> GetAllBlogsAsync(bool includeDeleted = false);
        Task<BlogDetailDto> GetBlogByIdAsync(int id, bool includeDeleted = false);
        Task<BlogDto> CreateBlogAsync(CreateBlogDto dto);
        Task UpdateBlogAsync(UpdateBlogDto dto);
        Task DeleteBlogAsync(int id, bool hardDelete = false);
        Task RestoreBlogAsync(int id);
        Task<IEnumerable<BlogDto>> GetBlogsByCategoryAsync(int categoryId);
        Task<IEnumerable<BlogDto>> SearchBlogsAsync(string searchTerm, bool includeDeleted = false);
    }
}