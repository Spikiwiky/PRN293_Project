using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EcommerceFrontend.Web.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IProductService productService, ILogger<IndexModel> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public List<ProductDTO> Products { get; set; } = new();
    public string? ErrorMessage { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? Size { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? Color { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public decimal? MinPrice { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public decimal? MaxPrice { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    
    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 12;

    public async Task OnGetAsync()
    {
        try
        {
            _logger.LogInformation(
                "Loading products - SearchTerm: {SearchTerm}, Category: {Category}, Size: {Size}, Color: {Color}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}, Page: {Page}, PageSize: {PageSize}", 
                SearchTerm, Category, Size, Color, MinPrice, MaxPrice, PageNumber, PageSize);

            if (!string.IsNullOrEmpty(SearchTerm) || !string.IsNullOrEmpty(Category) || 
                !string.IsNullOrEmpty(Size) || !string.IsNullOrEmpty(Color) || 
                MinPrice.HasValue || MaxPrice.HasValue)
            {
                Products = await _productService.SearchProductsAsync(
                    name: SearchTerm,
                    category: Category,
                    size: Size,
                    color: Color,
                    minPrice: MinPrice,
                    maxPrice: MaxPrice,
                    page: PageNumber,
                    pageSize: PageSize);
            }
            else
            {
                Products = await _productService.GetAllProductsAsync(PageNumber, PageSize);
            }

            if (Products == null || !Products.Any())
            {
                _logger.LogInformation("No products found for the given criteria");
                ErrorMessage = "No products found.";
                Products = new List<ProductDTO>();
            }
            else
            {
                _logger.LogInformation("Successfully loaded {Count} products", Products.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products");
            ErrorMessage = "An error occurred while loading products. Please try again later.";
            Products = new List<ProductDTO>();
        }
    }
} 