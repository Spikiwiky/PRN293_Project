using EcommerceFrontend.Web.Models.DTOs;
using System.Net.Http;
using System.Text.Json;

namespace EcommerceFrontend.Web.Services.Blog
{

        public class BlogService : IBlogService
        {
            private readonly IHttpClientFactory _httpClientFactory;
            private readonly ILogger<BlogService> _logger;
            private readonly IConfiguration _configuration;
            private const string BaseEndpoint = "/api/blog";

            public BlogService(IHttpClientFactory httpClientFactory, ILogger<BlogService> logger, IConfiguration configuration)
            {
                _httpClientFactory = httpClientFactory;
                _logger = logger;
                _configuration = configuration;
            }


            private HttpClient CreateClient()
            {
                return _httpClientFactory.CreateClient("MyAPI");
            }

            public async Task<List<BlogDto>> GetBlogsAsync()
            {
                try
                {
                    var client = CreateClient();
                    var response = await client.GetAsync($"{BaseEndpoint}/all");
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var blogs = System.Text.Json.JsonSerializer.Deserialize<List<BlogDto>>(json,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return blogs ?? new List<BlogDto>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching blog list");
                    throw;
                }
            }

            public async Task<BlogDto?> GetBlogByIdAsync(int id)
            {
                try
                {
                    var client = CreateClient();
                    var response = await client.GetAsync($"{BaseEndpoint}/{id}");
                    if (!response.IsSuccessStatusCode) return null;

                    var json = await response.Content.ReadAsStringAsync();
                    var blog = System.Text.Json.JsonSerializer.Deserialize<BlogDto>(json,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return blog;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to fetch blog with ID {id}");
                    throw;
                }
            }

            public async Task<bool> CreateBlogAsync(BlogDto blog)
            {
                try
                {
                    var client = CreateClient();
                    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(blog), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(BaseEndpoint, content);
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create blog");
                    throw;
                }
            }

            public async Task<bool> UpdateBlogAsync(BlogDto blog)
            {
                try
                {
                    var client = CreateClient();
                    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(blog), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PutAsync($"{BaseEndpoint}/{blog.BlogId}", content);
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to update blog with ID {blog.BlogId}");
                    throw;
                }
            }

            public async Task<bool> DeleteBlogAsync(int id)
            {
                try
                {
                    var client = CreateClient();
                    var response = await client.DeleteAsync($"{BaseEndpoint}/{id}?confirm=true");
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to delete blog with ID {id}");
                    throw;
                }
            }
        }
    }


