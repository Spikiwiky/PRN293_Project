using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Services.Admin;

namespace EcommerceFrontend.Web.Pages.Admin.Products
{
    public class DeleteModel : PageModel
    {
        private readonly IAdminProductService _adminProductService;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IAdminProductService adminProductService, ILogger<DeleteModel> logger)
        {
            _adminProductService = adminProductService;
            _logger = logger;
        }

        [BindProperty]
        public AdminProductDto Product { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var product = await _adminProductService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                Product = product;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product {Id} for deletion", id);
                TempData["ErrorMessage"] = "Error loading product details. Please try again.";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var result = await _adminProductService.DeleteProductAsync(Product.ProductId);
                if (!result)
                {
                    TempData["ErrorMessage"] = "Product not found or already deleted.";
                    return RedirectToPage("./Index");
                }

                TempData["SuccessMessage"] = "Product deleted successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {Id}", Product.ProductId);
                TempData["ErrorMessage"] = "Error deleting product. Please try again.";
                return RedirectToPage("./Index");
            }
        }
    }
} 