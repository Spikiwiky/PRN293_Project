using EcommerceBackend.DataAccess.Models;

namespace EcommerceBackend.BusinessObject.Services.SaleService.BlogService
{
    public interface ISaleBlogService
    {
        // Blog
        Task<IEnumerable<Blog>> GetAllBlogsAsync();
        Task<Blog?> GetBlogByIdAsync(int id);
        Task<Blog> CreateBlogAsync(Blog blog);
        Task UpdateBlogAsync(Blog blog);
        Task DeleteBlogAsync(int id);

        // Category
        Task<IEnumerable<BlogCategory>> GetAllCategoriesAsync();
        Task<BlogCategory?> GetCategoryByIdAsync(int id);
        Task<BlogCategory> CreateCategoryAsync(BlogCategory category);
        Task<BlogCategory?> UpdateCategoryAsync(BlogCategory category);
        Task<bool> DeleteCategoryAsync(int id);

        // Comment
        Task<IEnumerable<BlogComment>> GetCommentsByBlogIdAsync(int blogId);
        Task<BlogComment> AddCommentAsync(BlogComment comment);
        Task DeleteCommentAsync(int commentId);
        Task UpdateCommentAsync(BlogComment comment);
    }
}
