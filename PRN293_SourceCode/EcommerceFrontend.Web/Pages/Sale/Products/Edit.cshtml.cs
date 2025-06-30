using EcommerceFrontend.Web.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;

namespace EcommerceFrontend.Web.Pages.Sale.Products
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        [BindProperty]
        public ProductModel Product { get; set; }

        public EditModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value ?? throw new ArgumentNullException(nameof(apiSettings), "ApiSettings is not configured.");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.GetAsync($"{_apiSettings.BaseUrl}/api/sale/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                Product = await response.Content.ReadFromJsonAsync<ProductModel>() ?? new ProductModel();
                if (Product.ProductId != id)
                {
                    Product.ProductId = id;
                }
                return Page();
            }
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Product?.ProductId == 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid Product ID.");
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("MyAPI");
                var updateDto = new
                {
                    Name = Product.Name,
                    Description = Product.Description,
                    ProductCategoryId = Product.ProductCategoryId,
                    Brand = Product.Brand,
                    BasePrice = Product.BasePrice,
                    AvailableAttributes = Product.AvailableAttributes,
                    Status = Product.Status,
                    IsDelete = Product.IsDelete,
                    ProductImages = Product.ProductImages,
                    Variants = Product.Variants
                };
                var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");
                var fullUrl = $"{_apiSettings.BaseUrl}/api/sale/products/update/{Product.ProductId}";
                Console.WriteLine($"Sending PUT to {fullUrl} with data: {await content.ReadAsStringAsync()}");
                var response = await client.PutAsync(fullUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Update successful, redirecting to Index.");
                    return RedirectToPage("/Sale/Products/Index");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Update failed. Status: {response.StatusCode}, Error: {error}");
                    ModelState.AddModelError(string.Empty, $"Failed to update product. Status code: {response.StatusCode}. Error: {error}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during update: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Error updating product: {ex.Message}");
                return Page();
            }
        }
    }
}