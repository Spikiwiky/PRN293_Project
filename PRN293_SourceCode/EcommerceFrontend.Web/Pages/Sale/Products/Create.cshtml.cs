using EcommerceFrontend.Web.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;

namespace EcommerceFrontend.Web.Pages.Sale.Products
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        [BindProperty]
        public CreateProductDto Product { get; set; } = new CreateProductDto
        {
            ProductImages = new List<ProductImageDto> { new ProductImageDto() },
            Variants = new List<ProductVariantDto> { new ProductVariantDto() }
        };

        public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>();
        }

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Group attributes từ tất cả variant
            var attributes = new Dictionary<string, List<string>>
    {
        { "size", Product.Variants.Select(v => v.Size).Distinct().ToList() },
        { "color", Product.Variants.Select(v => v.Color).Distinct().ToList() },
        { "material", Product.Variants.Select(v => v.Material).Distinct().ToList() },
        { "supplier", Product.Variants.Select(v => v.Supplier).Distinct().ToList() },
        { "xuất xứ", Product.Variants.Select(v => v.XuatXu).Distinct().ToList() }
    };

            var dto = new CreateProductDto
            {
                Name = Product.Name,
                Description = Product.Description,
                ProductCategoryId = Product.ProductCategoryId,
                Brand = Product.Brand,
                BasePrice = Product.BasePrice,
                AvailableAttributes = JsonSerializer.Serialize(attributes),
                Status = Product.Status,
                IsDelete = Product.IsDelete,
                ProductImages = string.IsNullOrEmpty(Product.ImageUrl)
                    ? new List<ProductImageDto>()
                    : new List<ProductImageDto> { new ProductImageDto { ImageUrl = Product.ImageUrl } },
                Variants = Product.Variants.Select(v => new ProductVariantDto
                {
                    Attributes = JsonSerializer.Serialize(new
                    {
                        size = v.Size,
                        color = v.Color,
                        material = v.Material,
                        supplier = v.Supplier,
                        xuấtxứ = v.XuatXu
                    }),
                    Variants = JsonSerializer.Serialize(v)
                }).ToList()
            };

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{_apiSettings.BaseUrl}/api/SaleProduct/products", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Error creating product: {error}");
            return Page();
        }

    }
}
