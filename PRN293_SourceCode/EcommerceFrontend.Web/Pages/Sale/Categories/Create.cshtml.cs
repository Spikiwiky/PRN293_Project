using EcommerceFrontend.Web.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace EcommerceFrontend.Web.Pages.Sale.Categories
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        [BindProperty]
        public CategoryModel Category { get; set; }

        public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>();
            Category = new CategoryModel();
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(Category), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_apiSettings.BaseUrl}/api/SaleProduct/categories", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
