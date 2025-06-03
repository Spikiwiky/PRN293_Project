using EcommerceFrontend.Web.Models.Admin;

namespace EcommerceFrontend.Web.Services.Admin.Blog
{
    public interface IAdminBlogService
    {
        Task<List<AdminBlogDto>> GetBlogsAsync();
        Task<AdminBlogDto> GetBlogByIdAsync(int id);
        Task CreateBlogAsync(AdminBlogDto blog);
        Task UpdateBlogAsync(AdminBlogDto blog);
        Task DeleteBlogAsync(int id);
    }

}
