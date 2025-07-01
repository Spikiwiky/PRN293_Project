using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceFrontend.Web.Pages.Admin.Products
{
    public class DetailsModel : PageModel
    {
        private readonly IProductService _productService;
        public ProductDTO? Product { get; set; }

        public DetailsModel(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _productService.GetProductByIdAsync(id);
            if (Product == null)
                return NotFound();
            return Page();
        }
    }
} 