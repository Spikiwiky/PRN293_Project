using EcommerceFrontend.Web.Models.DTOs;

namespace EcommerceFrontend.Web.Services.Blog
{
    public class BlogService
    {
        private readonly HttpClient _httpClient;

        public BlogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BlogDto>> GetBlogsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<BlogDto>>("api/blog/load");
            return response ?? new List<BlogDto>();
        }

        public async Task<BlogDto?> GetBlogDetailsAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<BlogDto>($"api/blog/{id}");
        }
    }
}
