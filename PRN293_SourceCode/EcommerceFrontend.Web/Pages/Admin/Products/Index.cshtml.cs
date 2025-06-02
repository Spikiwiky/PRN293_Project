using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Services.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceFrontend.Web.Pages.Admin.Products
{
    public class SearchFilters
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsFeatured { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly IAdminProductService _adminProductService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IAdminProductService adminProductService, ILogger<IndexModel> logger)
        {
            _adminProductService = adminProductService;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public SearchFilters SearchFilters { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }
        public List<AdminProductDto> Products { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation(
                    "Fetching products with parameters: name={Name}, category={Category}, size={Size}, color={Color}, minPrice={MinPrice}, maxPrice={MaxPrice}, startDate={StartDate}, endDate={EndDate}, page={Page}, pageSize={PageSize}",
                    SearchFilters.Name, SearchFilters.Category, SearchFilters.Size, SearchFilters.Color, 
                    SearchFilters.MinPrice, SearchFilters.MaxPrice, SearchFilters.StartDate, SearchFilters.EndDate, 
                    CurrentPage, PageSize);

                // Get total count for pagination
                var totalCount = await _adminProductService.GetTotalProductCountAsync();
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

                // Get products with filters
                Products = await _adminProductService.SearchProductsAsync(
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

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                TempData["Error"] = "An error occurred while fetching products. Please try again.";
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