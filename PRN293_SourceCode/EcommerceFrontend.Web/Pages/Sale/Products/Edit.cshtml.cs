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

        [BindProperty]
        public string AvailableSizes { get; set; }

        [BindProperty]
        public string AvailableColors { get; set; }

        [BindProperty]
        public string ImageUrl { get; set; }

        [BindProperty]
        public List<ProductVariantDisplayModel> VariantDisplays { get; set; } = new();

        public EditModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value ?? throw new ArgumentNullException(nameof(apiSettings));
        }

        public async Task<IActionResult> OnGetAsync([FromQuery] int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.GetAsync($"{_apiSettings.BaseUrl}/api/sale/products/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            Product = await response.Content.ReadFromJsonAsync<ProductModel>() ?? new ProductModel();

            // Parse AvailableAttributes safely
            if (!string.IsNullOrEmpty(Product.AvailableAttributes))
            {
                try
                {
                    var attributesDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(Product.AvailableAttributes);

                    if (attributesDict != null)
                    {
                        if (attributesDict.TryGetValue("size", out var sizeElement) && sizeElement.ValueKind == JsonValueKind.Array)
                            AvailableSizes = string.Join(",", sizeElement.EnumerateArray().Select(x => x.GetString()));

                        if (attributesDict.TryGetValue("color", out var colorElement) && colorElement.ValueKind == JsonValueKind.Array)
                            AvailableColors = string.Join(",", colorElement.EnumerateArray().Select(x => x.GetString()));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing AvailableAttributes: {ex.Message}");
                }
            }

            // Get ImageUrl
            ImageUrl = Product.ProductImages?.FirstOrDefault()?.ImageUrl ?? "";

            // Parse Variants
            VariantDisplays = new List<ProductVariantDisplayModel>();
            if (Product.Variants != null)
            {
                foreach (var v in Product.Variants)
                {
                    if (!string.IsNullOrEmpty(v.Variants))
                    {
                        try
                        {
                            var variantList = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(v.Variants);
                            if (variantList != null)
                            {
                                foreach (var item in variantList)
                                {
                                    VariantDisplays.Add(new ProductVariantDisplayModel
                                    {
                                        Size = item.TryGetValue("size", out var sz) ? sz.GetString() : "",
                                        Color = item.TryGetValue("color", out var cl) ? cl.GetString() : "",
                                        Price = item.TryGetValue("price", out var pr) ? pr.GetDecimal() : 0,
                                        Stock = item.TryGetValue("stock", out var st) ? st.GetInt32() : 0
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing variant: {ex.Message}");
                        }
                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            Console.WriteLine($"[OnPost] Submitting update for ProductId = {Product?.ProductId}");

            

            // Serialize AvailableAttributes
            var attributesDict = new Dictionary<string, List<string>>
            {
                {
                    "size",
                    AvailableSizes?
                        .Split(new[] { ',', '.' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList() ?? new List<string>()
                },
                {
                    "color",
                    AvailableColors?
                        .Split(new[] { ',', '.' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(c => c.Trim())
                        .Where(c => !string.IsNullOrEmpty(c))
                        .ToList() ?? new List<string>()
                }
            };

            var availableAttributesJson = JsonSerializer.Serialize(attributesDict);

            // Serialize Variants
            var variantsList = VariantDisplays.Select(v => new Dictionary<string, object>
            {
                { "size", v.Size },
                { "color", v.Color },
                { "price", v.Price },
                { "stock", v.Stock }
            }).ToList();

            var client = _httpClientFactory.CreateClient("MyAPI");

            // Prepare DTO
            var variantsJson = JsonSerializer.Serialize(variantsList);

            // Prepare DTO
            var updateDto = new
            {
                ProductId = Product.ProductId,
                Name = Product.Name,
                Description = Product.Description,
                ProductCategoryId = Product.ProductCategoryId,
                Brand = Product.Brand,
                BasePrice = Product.BasePrice,
                AvailableAttributes = availableAttributesJson,
                Status = Product.Status,
                IsDelete = Product.IsDelete,
                ProductImages = string.IsNullOrEmpty(ImageUrl)
                    ? new List<object>()
                    : new List<object> { new { ImageUrl } },
                Variants = new List<object>
    {
        new
        {
            Attributes = availableAttributesJson, // Gửi thêm attributes
            Variants = variantsJson
        }
    }
            };

            var jsonContent = JsonSerializer.Serialize(updateDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_apiSettings.BaseUrl}/api/sale/products/{Product.ProductId}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("/Sale/Products/Index");

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Update failed: {error}");
            return Page();
        }
    }

    public class ProductVariantDisplayModel
    {
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
