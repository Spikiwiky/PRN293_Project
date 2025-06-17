using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Models.Admin;

namespace EcommerceFrontend.Web.Services.Admin.Blog
{
    public class AdminBlogService : IAdminBlogService
    {
        private readonly HttpClient _httpClient;

        public AdminBlogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AdminBlogDto>> GetBlogsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<AdminBlogDto>>("api/blog/all");
            return response ?? new List<AdminBlogDto>();
        }

        public async Task<AdminBlogDto> GetBlogByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<AdminBlogDto>($"api/blog/{id}");
        }

        public async Task CreateBlogAsync(AdminBlogDto blog)
        {
            var response = await _httpClient.PostAsJsonAsync("api/blog", blog);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateBlogAsync(AdminBlogDto blog)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/blog/{blog.BlogId}", blog);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBlogAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/blog/{id}?confirm=true");
            response.EnsureSuccessStatusCode();
        }
    }
}
