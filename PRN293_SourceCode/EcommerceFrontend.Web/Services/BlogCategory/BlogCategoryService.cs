using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EcommerceFrontend.Web.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EcommerceFrontend.Web.Services.Admin.BlogCategory
{
    public class BlogCategoryService : IBlogCategoryServiceWeb
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BlogCategoryService> _logger;
        private const string BaseEndpoint = "/api/blogcategories";

        public BlogCategoryService(
            IHttpClientFactory httpClientFactory,
            ILogger<BlogCategoryService> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<IEnumerable<BlogCategoryDto>> GetAllCategoriesAsync(bool includeDeleted = false)
        {
            try
            {
                var client = CreateClient();
                var response = await client.GetAsync($"{BaseEndpoint}?includeDeleted={includeDeleted}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<BlogCategoryDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<BlogCategoryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching blog categories");
                throw;
            }
        }

        public async Task<BlogCategoryDto> GetCategoryByIdAsync(int id, bool includeDeleted = false)
        {
            try
            {
                var client = CreateClient();
                var response = await client.GetAsync($"{BaseEndpoint}/{id}?includeDeleted={includeDeleted}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<BlogCategoryDto>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to fetch blog category with ID {id}");
                throw;
            }
        }

        public async Task<BlogCategoryDto> CreateCategoryAsync(CreateBlogCategoryDto dto)
        {
            try
            {
                var client = CreateClient();
                var content = new StringContent(
                    JsonSerializer.Serialize(dto),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(BaseEndpoint, content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<BlogCategoryDto>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create blog category");
                throw;
            }
        }

        public async Task UpdateCategoryAsync(UpdateBlogCategoryDto dto)
        {
            try
            {
                var client = CreateClient();
                var content = new StringContent(
                    JsonSerializer.Serialize(dto),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PutAsync($"{BaseEndpoint}/{dto.BlogCategoryId}", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update blog category with ID {dto.BlogCategoryId}");
                throw;
            }
        }

        public async Task DeleteCategoryAsync(int id, bool hardDelete = false)
        {
            try
            {
                var client = CreateClient();
                var response = await client.DeleteAsync($"{BaseEndpoint}/{id}?hardDelete={hardDelete}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete blog category with ID {id}");
                throw;
            }
        }

        public async Task RestoreCategoryAsync(int id)
        {
            try
            {
                var client = CreateClient();
                var response = await client.PatchAsync($"{BaseEndpoint}/{id}/restore", null);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to restore blog category with ID {id}");
                throw;
            }
        }
    }
}