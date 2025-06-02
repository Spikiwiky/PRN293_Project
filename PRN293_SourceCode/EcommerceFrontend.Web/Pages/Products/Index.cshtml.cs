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
    public int PageNumber { get; set; } = 1;
    
    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 12;

    public async Task OnGetAsync()
    {
        try
        {
            _logger.LogInformation("Received request - SearchTerm: {SearchTerm}, Category: {Category}", SearchTerm, Category);

            // Only use search if we have a search term. For category-only filtering, use the load endpoint
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                _logger.LogInformation("Using search with term: {SearchTerm}", SearchTerm);
                Products = await _productService.GetAllProductsAsync(PageNumber, PageSize);
                
                // Filter the results in memory if we have a category
                if (!string.IsNullOrEmpty(Category))
                {
                    Products = Products.Where(p => p.ProductCategoryTitle == Category).ToList();
                    _logger.LogInformation("Filtered to {Count} products in category {Category}", Products.Count, Category);
                }
            }
            else if (!string.IsNullOrEmpty(Category))
            {
                _logger.LogInformation("Getting products for category: {Category}", Category);
                Products = await _productService.GetAllProductsAsync(PageNumber, PageSize);
                Products = Products.Where(p => p.ProductCategoryTitle == Category).ToList();
                _logger.LogInformation("Found {Count} products in category {Category}", Products.Count, Category);
            }
            else
            {
                _logger.LogInformation("Getting all products without filters");
                Products = await _productService.GetAllProductsAsync(PageNumber, PageSize);
                _logger.LogInformation("Retrieved {Count} products total", Products.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products");
            ErrorMessage = "Failed to load products. Please try again later.";
        }
    }
} 