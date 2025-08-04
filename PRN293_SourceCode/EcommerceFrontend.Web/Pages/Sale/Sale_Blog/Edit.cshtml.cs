using EcommerceFrontend.Web.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Blog
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        [BindProperty]
        public BlogUpdateDto Blog { get; set; } = new();

        [BindProperty]
        public bool RemoveImage { get; set; }

        [BindProperty]
        public string? BlogImageUrl { get; set; }

        public EditModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            Blog = await client.GetFromJsonAsync<BlogUpdateDto>($"{_apiSettings.BaseUrl}/api/saleblog/{id}");
            if (Blog == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
                return Page();

            var client = _httpClientFactory.CreateClient("MyAPI");

            var payload = new
            {
                BlogCategoryId = Blog.BlogCategoryId,
                BlogTittle = Blog.BlogTittle,
                Tags = Blog.Tags,
                BlogContent = Blog.BlogContent,
                BlogSummary = Blog.BlogSummary,
                IsPublished = Blog.IsPublished,
                RemoveImage = RemoveImage,
                BlogImageUrl = BlogImageUrl
            };

            var response = await client.PutAsJsonAsync($"{_apiSettings.BaseUrl}/api/saleblog/{id}", payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError("", "Không thể cập nhật blog");
            return Page();
        }
    }
}
