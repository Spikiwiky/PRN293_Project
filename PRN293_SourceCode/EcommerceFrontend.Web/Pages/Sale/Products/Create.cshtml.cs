using EcommerceFrontend.Web.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using EcommerceBackend.BusinessObject.dtos.SaleDto;

namespace EcommerceFrontend.Web.Pages.Sale.Products
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        [BindProperty]
        public CreateProductInputModel Product { get; set; }

        public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>();
        }

        public IActionResult OnGet()
        {
            if (Product == null)
            {
                Product = new CreateProductInputModel
                {
                    Variants = new List<ProductVariantInputModel>
            {
                new ProductVariantInputModel()
            },
                    AvailableSizes = new List<string>(),
                    AvailableColors = new List<string>()
                };
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var availableAttributesObj = new Dictionary<string, List<string>>
    {
        {
            "size",
            Product.AvailableSizes
                .SelectMany(s => s.Split(new[] { ',', '.' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList()
        },
        {
            "color",
            Product.AvailableColors
                .SelectMany(s => s.Split(new[] { ',', '.' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrEmpty(c))
                .ToList()
        }
    };
            var availableAttributesJson = JsonSerializer.Serialize(availableAttributesObj);
             
            var variantsList = Product.Variants.Select(v => new Dictionary<string, object>
    {
        { "size", v.Size },
        { "color", v.Color },
        { "price", v.Price },
        { "stock", v.Stock }
    }).ToList();
            var variantsJson = JsonSerializer.Serialize(variantsList);

            var dto = new CreateProductDto
            {
                Name = Product.Name,
                Description = Product.Description,
                ProductCategoryId = Product.ProductCategoryId,
                Brand = Product.Brand,
                BasePrice = Product.BasePrice,
                AvailableAttributes = availableAttributesJson,
                Status = Product.Status,
                IsDelete = Product.IsDelete,
                ProductImages = string.IsNullOrEmpty(Product.ImageUrl)
                    ? new List<ProductImageDto>()
                    : new List<ProductImageDto> { new ProductImageDto { ImageUrl = Product.ImageUrl } },
                Variants = new List<ProductVariantDto>
    {
        new ProductVariantDto
        {
            Attributes = availableAttributesJson, 
            Variants = JsonSerializer.Serialize(variantsList)
        }
    }
            };

            var json = JsonSerializer.Serialize(dto);

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_apiSettings.BaseUrl}/api/SaleProduct/products", content);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Error creating product: {error}");
            return Page();
        }
    }
}
