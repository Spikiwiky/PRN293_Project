using EcommerceFrontend.Web.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;

namespace EcommerceFrontend.Web.Pages.Sale.Categories
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        [BindProperty]
        public CategoryModel Category { get; set; }

        public EditModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiSettings.BaseUrl}/api/sale/categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                Category = await response.Content.ReadFromJsonAsync<CategoryModel>();
                return Page();
            }
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(Category), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiSettings.BaseUrl}/api/sale/categories/{Category.ProductCategoryId}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }
            ModelState.AddModelError(string.Empty, "Failed to update category.");
            return Page();
        }
    }
}
