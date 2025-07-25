using EcommerceFrontend.Web.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Blog
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public BlogDetailDto Blog { get; set; } = new();
        [BindProperty] public CommentDto NewComment { get; set; } = new();

        public DetailsModel(IHttpClientFactory clientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = clientFactory;
            _apiSettings = apiSettings.Value ?? throw new ArgumentNullException(nameof(apiSettings), "ApiSettings is not configured.");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            await LoadBlogAsync(id);
            return Page();
        }

        private async Task LoadBlogAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            Blog = await client.GetFromJsonAsync<BlogDetailDto>($"{_apiSettings.BaseUrl}/api/saleblog/{id}") ?? new BlogDetailDto();

            // Tăng view count
            await client.GetAsync($"{_apiSettings.BaseUrl}/api/saleblog/view/{id}");
        }

        public async Task<IActionResult> OnPostAddCommentAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await LoadBlogAsync(id);
                return Page();
            }

            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.PostAsJsonAsync($"{_apiSettings.BaseUrl}/api/saleblog/{id}/comments", NewComment);

            if (response.IsSuccessStatusCode)
                return RedirectToPage(new { id });

            ModelState.AddModelError("", "Không thể thêm bình luận");
            await LoadBlogAsync(id);
            return Page();
        }

    }
}
