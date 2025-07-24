using EcommerceFrontend.Web.Models.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceFrontend.Web.Services.Admin.Blog
{
    public interface IAdminBlogService
    {
        #region Blog Methods

        // Gets paginated list of blogs
        Task<PaginatedResponse<AdminBlogDto>> GetBlogsAsync(int page = 1, int pageSize = 10, bool includeDeleted = false);

        // Gets a single blog with full details including comments
        Task<AdminBlogDetailDto> GetBlogByIdAsync(int id, bool includeDeleted = false);

        // Creates a new blog and returns the created entity
        Task<AdminBlogDto> CreateBlogAsync(AdminCreateBlogDto blog);

        // Updates an existing blog
        Task UpdateBlogAsync(AdminUpdateBlogDto blog);

        // Deletes a blog (soft delete by default)
        Task DeleteBlogAsync(int id, bool hardDelete = false);

        // Restores a previously deleted blog
        Task RestoreBlogAsync(int id);

        // Searches blogs by title/content
        Task<IEnumerable<AdminBlogDto>> SearchBlogsAsync(string searchTerm, bool includeDeleted = false);

        Task<List<BlogCategoryDto>> GetCategoriesAsync();

        #endregion

        #region Comment Methods

        // Gets paginated comments for a specific blog
        Task<PaginatedResponse<AdminCommentDto>> GetCommentsByBlogAsync(int blogId, int page = 1, int pageSize = 10, bool includeDeleted = false);

        // Gets a single comment by ID
        Task<AdminCommentDto> GetCommentByIdAsync(int commentId);

        // Creates a new comment and returns the created entity
        Task<AdminCommentDto> CreateCommentAsync(AdminCreateCommentDto comment);

        // Updates an existing comment
        Task UpdateCommentAsync(AdminUpdateCommentDto comment);

        // Deletes a comment (soft delete by default)
        Task DeleteCommentAsync(int commentId, bool hardDelete = false);

        // Gets comment count for a specific blog
        Task<int> GetCommentCountForBlogAsync(int blogId);

        #endregion
    }
}