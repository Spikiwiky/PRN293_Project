using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceFrontend.Web.Pages.Admin.Products
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;
        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; }
        public ProductSearchParams? SearchParams { get; set; }

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        public async Task OnGetAsync(int page = 1, int pageSize = 20, string? name = null, string? category = null)
        {
            SearchParams = new ProductSearchParams
            {
                Name = name,
                Category = category
            };
            Products = await _productService.GetAllProductsAsync(page, pageSize, name, category);
            CurrentPage = page;
            PageSize = pageSize;
            int totalProducts = await _productService.GetTotalProductsCountAsync(name, category);
            TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);
        }
    }
} 