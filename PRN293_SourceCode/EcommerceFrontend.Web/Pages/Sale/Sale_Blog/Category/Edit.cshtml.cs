using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using EcommerceFrontend.Web.Models.Sale;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Blog.Category
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        [BindProperty]
        public BlogCategoryDto Category { get; set; } = new();

        public EditModel(IHttpClientFactory clientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = clientFactory;
            _apiSettings = apiSettings.Value;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            Category = await client.GetFromJsonAsync<BlogCategoryDto>($"{_apiSettings.BaseUrl}/api/saleblog/categories/{id}");
            if (Category == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.PutAsJsonAsync(
                $"api/saleblog/categories/{Category.BlogCategoryId}",
                Category);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError("", "Không thể cập nhật danh mục");
            return Page();
        }

    }
}
