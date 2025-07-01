using EcommerceFrontend.Web.Models.DTOs;

namespace EcommerceFrontend.Web.Services.Blog
{
    public interface IBlogService
    {
        Task<List<BlogDto>> GetBlogsAsync();
        Task<BlogDto?> GetBlogByIdAsync(int id);
        Task<bool> CreateBlogAsync(BlogDto blog);
        Task<bool> UpdateBlogAsync(BlogDto blog);
        Task<bool> DeleteBlogAsync(int id);
    }
}
