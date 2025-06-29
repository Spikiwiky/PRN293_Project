using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceFrontend.Web.Models.Sale;
using EcommerceFrontend.Web.Services.Sale;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceFrontend.Web.Pages.Sale.Products
{
    public class SearchFilters
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsFeatured { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly ISaleProductService _saleProductService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ISaleProductService saleProductService, ILogger<IndexModel> logger)
        {
            _saleProductService = saleProductService;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public SearchFilters SearchFilters { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }
        public List<SaleProductDto> Products { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation(
                    "Fetching products with parameters: name={Name}, category={Category}, size={Size}, color={Color}, minPrice={MinPrice}, maxPrice={MaxPrice}, startDate={StartDate}, endDate={EndDate}, page={Page}, pageSize={PageSize}",
                    SearchFilters.Name, SearchFilters.Category, SearchFilters.Size, SearchFilters.Color, 
                    SearchFilters.MinPrice, SearchFilters.MaxPrice, SearchFilters.StartDate, SearchFilters.EndDate, 
                    CurrentPage, PageSize);

                // First try to get the total count
                try
                {
                    var totalCount = await _saleProductService.GetTotalProductCountAsync();
                    TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting total product count");
                    TotalPages = 1; // Set default value
                }

                // Then try to get the products
                Products = await _saleProductService.SearchProductsAsync(
                    name: SearchFilters.Name,
                    category: SearchFilters.Category,
                    size: SearchFilters.Size,
                    color: SearchFilters.Color,
                    minPrice: SearchFilters.MinPrice,
                    maxPrice: SearchFilters.MaxPrice,
                    startDate: SearchFilters.StartDate,
                    endDate: SearchFilters.EndDate,
                    isFeatured: SearchFilters.IsFeatured,
                    page: CurrentPage,
                    pageSize: PageSize
                );

                if (!Products.Any())
                {
                    TempData["Warning"] = "No products found matching your criteria.";
                }

                return Page();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while fetching products: {Message}", ex.Message);
                TempData["Error"] = $"Error connecting to the product service. Please try again later. ({ex.Message})";
                Products = new List<SaleProductDto>();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching products");
                TempData["Error"] = "An unexpected error occurred while fetching products. Please try again.";
                Products = new List<SaleProductDto>();
                return Page();
            }
        }

        public Dictionary<string, string> GetPageRouteData(int pageNumber)
        {
            var routeData = new Dictionary<string, string>
            {
                { "CurrentPage", pageNumber.ToString() }
            };

            if (!string.IsNullOrEmpty(SearchFilters.Name))
                routeData.Add("SearchFilters.Name", SearchFilters.Name);
            
            if (!string.IsNullOrEmpty(SearchFilters.Category))
                routeData.Add("SearchFilters.Category", SearchFilters.Category);
            
            if (!string.IsNullOrEmpty(SearchFilters.Size))
                routeData.Add("SearchFilters.Size", SearchFilters.Size);
            
            if (!string.IsNullOrEmpty(SearchFilters.Color))
                routeData.Add("SearchFilters.Color", SearchFilters.Color);
            
            if (SearchFilters.MinPrice.HasValue)
                routeData.Add("SearchFilters.MinPrice", SearchFilters.MinPrice.Value.ToString());
            
            if (SearchFilters.MaxPrice.HasValue)
                routeData.Add("SearchFilters.MaxPrice", SearchFilters.MaxPrice.Value.ToString());
            
            if (SearchFilters.StartDate.HasValue)
                routeData.Add("SearchFilters.StartDate", SearchFilters.StartDate.Value.ToString("yyyy-MM-dd"));
            
            if (SearchFilters.EndDate.HasValue)
                routeData.Add("SearchFilters.EndDate", SearchFilters.EndDate.Value.ToString("yyyy-MM-dd"));
            
            if (SearchFilters.IsFeatured.HasValue)
                routeData.Add("SearchFilters.IsFeatured", SearchFilters.IsFeatured.Value.ToString());

            return routeData;
        }
    }
} 