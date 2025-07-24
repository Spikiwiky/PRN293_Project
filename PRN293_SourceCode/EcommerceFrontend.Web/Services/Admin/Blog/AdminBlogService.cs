using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Models.Admin;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace EcommerceFrontend.Web.Services.Admin.Blog
{
    public class AdminBlogService : IAdminBlogService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AdminBlogService> _logger;
        private const string BlogBaseEndpoint = "api/blog";
        private const string CommentBaseEndpoint = "api/comment";

        public AdminBlogService(
            IHttpClientFactory httpClientFactory,
            ILogger<AdminBlogService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            // Add any default headers or configuration here if needed
            return client;
        }

        #region Blog Methods

        public async Task<PaginatedResponse<AdminBlogDto>> GetBlogsAsync(
            int page = 1,
            int pageSize = 10,
            bool includeDeleted = false)
        {
            try
            {
                using var client = CreateClient();
                var response = await client.GetFromJsonAsync<PaginatedResponse<AdminBlogDto>>(
                    $"{BlogBaseEndpoint}?page={page}&pageSize={pageSize}&includeDeleted={includeDeleted}");

                return response ?? new PaginatedResponse<AdminBlogDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching blogs (Page: {Page}, PageSize: {PageSize})", page, pageSize);
                return new PaginatedResponse<AdminBlogDto>();
            }
        }

        public async Task<AdminBlogDetailDto> GetBlogByIdAsync(int id, bool includeDeleted = false)
        {
            try
            {
                using var client = CreateClient();
                return await client.GetFromJsonAsync<AdminBlogDetailDto>(
                    $"{BlogBaseEndpoint}/{id}?includeDeleted={includeDeleted}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching blog with ID {BlogId}", id);
                throw;
            }
        }

        public async Task<AdminBlogDto> CreateBlogAsync(AdminCreateBlogDto blog)
        {
            try
            {
                using var client = CreateClient();
                var response = await client.PostAsJsonAsync(BlogBaseEndpoint, blog);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<AdminBlogDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating blog");
                throw;
            }
        }

        public async Task UpdateBlogAsync(AdminUpdateBlogDto blog)
        {
            try
            {
                using var client = CreateClient();
                var response = await client.PutAsJsonAsync($"{BlogBaseEndpoint}/{blog.BlogId}", blog);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blog with ID {BlogId}", blog.BlogId);
                throw;
            }
        }

        public async Task DeleteBlogAsync(int id, bool hardDelete = false)
        {
            try
            {
                using var client = CreateClient();
                var response = await client.DeleteAsync($"{BlogBaseEndpoint}/{id}?hardDelete={hardDelete}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blog with ID {BlogId}", id);
                throw;
            }
        }

        public async Task RestoreBlogAsync(int id)
        {
            try
            {
                using var client = CreateClient();
                var response = await client.PatchAsync($"{BlogBaseEndpoint}/{id}/restore", null);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring blog with ID {BlogId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<AdminBlogDto>> SearchBlogsAsync(string searchTerm, bool includeDeleted = false)
        {
            try
            {
                using var client = CreateClient();
                return await client.GetFromJsonAsync<IEnumerable<AdminBlogDto>>(
                    $"{BlogBaseEndpoint}/search?term={searchTerm}&includeDeleted={includeDeleted}")
                    ?? Enumerable.Empty<AdminBlogDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching blogs with term '{SearchTerm}'", searchTerm);
                return Enumerable.Empty<AdminBlogDto>();
            }
        }

        public async Task<List<BlogCategoryDto>> GetCategoriesAsync()
        {
            try
            {
                using var client = CreateClient();
                return await client.GetFromJsonAsync<List<BlogCategoryDto>>("api/blogcategories")
                    ?? new List<BlogCategoryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching blog categories");
                return new List<BlogCategoryDto>();
            }
        }

        #endregion

        #region Comment Methods

        public async Task<PaginatedResponse<AdminCommentDto>> GetCommentsByBlogAsync(
            int blogId,
            int page = 1,
            int pageSize = 10,
            bool includeDeleted = false)
        {
            try
            {
                using var client = CreateClient();
                return await client.GetFromJsonAsync<PaginatedResponse<AdminCommentDto>>(
                    $"{CommentBaseEndpoint}/blog/{blogId}?page={page}&pageSize={pageSize}&includeDeleted={includeDeleted}")
                    ?? new PaginatedResponse<AdminCommentDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching comments for blog {BlogId}", blogId);
                return new PaginatedResponse<AdminCommentDto>();
            }
        }

        public async Task<AdminCommentDto> GetCommentByIdAsync(int commentId)
        {
            try
            {
                using var client = CreateClient();
                return await client.GetFromJsonAsync<AdminCommentDto>($"{CommentBaseEndpoint}/{commentId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching comment with ID {CommentId}", commentId);
                throw;
            }
        }

        public async Task<AdminCommentDto> CreateCommentAsync(AdminCreateCommentDto comment)
        {
            try
            {
                using var client = CreateClient();
                var response = await client.PostAsJsonAsync(CommentBaseEndpoint, comment);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<AdminCommentDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment for blog {BlogId}", comment.BlogId);
                throw;
            }
        }

        public async Task UpdateCommentAsync(AdminUpdateCommentDto comment)
        {
            try
            {
                using var client = CreateClient();
                var response = await client.PutAsJsonAsync(CommentBaseEndpoint, comment);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment with ID {CommentId}", comment.CommentId);
                throw;
            }
        }

        public async Task DeleteCommentAsync(int commentId, bool hardDelete = false)
        {
            try
            {
                using var client = CreateClient();
                var response = await client.DeleteAsync($"{CommentBaseEndpoint}/{commentId}?hardDelete={hardDelete}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment with ID {CommentId}", commentId);
                throw;
            }
        }

        public async Task<int> GetCommentCountForBlogAsync(int blogId)
        {
            try
            {
                using var client = CreateClient();
                return await client.GetFromJsonAsync<int>($"{CommentBaseEndpoint}/blog/{blogId}/count");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment count for blog {BlogId}", blogId);
                return 0;
            }
        }

        #endregion
    }
}