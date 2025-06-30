using EcommerceFrontend.Web.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace EcommerceFrontend.Web.Pages.Sale.Products
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public List<ProductModel> Products { get; set; } = new List<ProductModel>();  

        public IndexModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value ?? throw new ArgumentNullException(nameof(apiSettings), "ApiSettings is not configured.");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MyAPI");
                var fullUrl = $"{_apiSettings.BaseUrl}/api/sale/products";
                Console.WriteLine($"Requesting: {fullUrl}"); 
                var response = await client.GetAsync(fullUrl);
                if (response.IsSuccessStatusCode)
                {
                    Products = await response.Content.ReadFromJsonAsync<List<ProductModel>>() ?? new List<ProductModel>();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Failed to fetch products. Status code: {response.StatusCode}");
                    Products = new List<ProductModel>(); // Đảm bảo khởi tạo lại
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error fetching products: {ex.Message}");
                Products = new List<ProductModel>(); // Đảm bảo khởi tạo lại
            }
            return Page();
        }
    }
}
