using EcommerceBackend.BusinessObject.Services.SaleService;
using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Models.Sale;
using EcommerceFrontend.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace EcommerceFrontend.Web.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductService _productService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiSettings _apiSettings;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IProductService productService, IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ILogger<IndexModel> logger)
    {
        _productService = productService;
        _httpClientFactory = httpClientFactory;
        _apiSettings = apiSettings.Value ?? throw new ArgumentNullException(nameof(apiSettings), "ApiSettings is not configured.");
        _logger = logger;
    }

    public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
    public ProductSearchParams SearchParams { get; set; } = new ProductSearchParams();
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public string? ErrorMessage { get; set; }
    public List<CategoryModel> Categories { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(
        string? name = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int page = 1)
    {
        try
        {
            SearchParams = new ProductSearchParams
            {
                Name = name,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Page = page,
                PageSize = PageSize
            };
             // search b�ng  parameter
           
            Products = await _productService.SearchProductsAsync(SearchParams);

            CurrentPage = page;

            // Get total count of products for pagination
            var totalProducts = await _productService.GetTotalProductsCountAsync(
                name: name,
                category: category,
                minPrice: minPrice,
                maxPrice: maxPrice
            );

            TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

            var client = _httpClientFactory.CreateClient("MyAPI");
            var fullUrl = $"{_apiSettings.BaseUrl}/api/sale/categories";
            var response = await client.GetAsync(fullUrl);
            if (response.IsSuccessStatusCode)
            {
                Categories = await response.Content.ReadFromJsonAsync<List<CategoryModel>>() ?? new List<CategoryModel>();
            }
            else
            {
                Categories = new List<CategoryModel>();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products");
            ErrorMessage = "An error occurred while loading products. Please try again later.";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
           
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product");
            ErrorMessage = "An error occurred while deleting the product.";
            return Page();
        }
    }
} 