using EcommerceFrontend.Web.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace EcommerceFrontend.Web.Pages.Sale.Categories
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();

        public IndexModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value ?? throw new ArgumentNullException(nameof(apiSettings), "ApiSettings is not configured.");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var url = $"{_apiSettings.BaseUrl}/api/sale/categories/{id}";

            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            { 
                return RedirectToPage();
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Failed to delete category. Status code: {response.StatusCode}");
                await LoadCategoriesAsync();
                return Page();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MyAPI");
                var fullUrl = $"{_apiSettings.BaseUrl}/api/sale/categories";
                var response = await client.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    Categories = await response.Content.ReadFromJsonAsync<List<CategoryModel>>() ?? new List<CategoryModel>();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Failed to fetch categories. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error fetching categories: {ex.Message}");
            }
        }
    }
}
