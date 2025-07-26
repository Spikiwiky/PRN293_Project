using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using EcommerceFrontend.Web.Models.Sale;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Blog.Category
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        [BindProperty]
        public BlogCategoryDto Category { get; set; } = new();

        public CreateModel(IHttpClientFactory clientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = clientFactory;
            _apiSettings = apiSettings.Value;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.PostAsJsonAsync($"{_apiSettings.BaseUrl}/api/saleblog/categories", Category);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError("", "Không thể thêm danh mục");
            return Page();
        }
    }
}
