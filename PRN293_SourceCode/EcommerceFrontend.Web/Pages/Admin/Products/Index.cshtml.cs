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

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        public async Task OnGetAsync(int page = 1, int pageSize = 10)
        {
            Products = await _productService.GetAllProductsAsync(page, pageSize);
        }
    }
} 