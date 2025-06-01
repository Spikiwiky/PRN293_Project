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
            if (!string.IsNullOrEmpty(SearchTerm) || !string.IsNullOrEmpty(Category))
            {
                Products = await _productService.SearchProductsAsync(
                    name: SearchTerm,
                    category: Category,
                    page: PageNumber,
                    pageSize: PageSize
                );
            }
            else
            {
                Products = await _productService.GetAllProductsAsync(PageNumber, PageSize);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products");
            ErrorMessage = "Failed to load products. Please try again later.";
        }
    }
} 