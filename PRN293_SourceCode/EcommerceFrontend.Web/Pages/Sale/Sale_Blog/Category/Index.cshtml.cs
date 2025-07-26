using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using EcommerceFrontend.Web.Models.Sale;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Blog.Category
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public List<BlogCategoryDto> Categories { get; set; } = new();

        public IndexModel(IHttpClientFactory clientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = clientFactory;
            _apiSettings = apiSettings.Value;
        }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            Categories = await client.GetFromJsonAsync<List<BlogCategoryDto>>($"{_apiSettings.BaseUrl}/api/saleblog/categories")
                         ?? new List<BlogCategoryDto>();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            await client.DeleteAsync($"{_apiSettings.BaseUrl}/api/saleblog/categories/{id}");
            return RedirectToPage();
        }
    }
}
