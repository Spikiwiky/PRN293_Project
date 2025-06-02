using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Services.Admin;

namespace EcommerceFrontend.Web.Pages.Admin.Products
{
    public class EditModel : PageModel
    {
        private readonly IAdminProductService _adminProductService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IAdminProductService adminProductService, ILogger<EditModel> logger)
        {
            _adminProductService = adminProductService;
            _logger = logger;
        }

        [BindProperty]
        public AdminProductUpdateDto Product { get; set; } = new();

        [BindProperty]
        public string? ImageUrlsInput { get; set; }

        public SelectList Categories { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var product = await _adminProductService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Get categories for dropdown
                var categories = await _adminProductService.GetCategoriesAsync();
                Categories = new SelectList(categories, "Id", "Name");

                // Map the product data to the update DTO
                Product = new AdminProductUpdateDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    ProductCategoryId = product.ProductCategoryId,
                    Category = product.Category,
                    Price = product.Price,
                    Size = product.Size,
                    Color = product.Color,
                    IsFeatured = product.IsFeatured,
                    Status = product.Status,
                    StockQuantity = product.StockQuantity
                };

                // Set image URLs input
                if (product.ImageUrls?.Any() == true)
                {
                    ImageUrlsInput = string.Join(",", product.ImageUrls);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product with ID {Id}", id);
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload categories on validation error
                var categories = await _adminProductService.GetCategoriesAsync();
                Categories = new SelectList(categories, "Id", "Name");
                return Page();
            }

            try
            {
                // Process image URLs from comma-separated input
                if (!string.IsNullOrEmpty(ImageUrlsInput))
                {
                    Product.ImageUrls = ImageUrlsInput.Split(',')
                        .Select(url => url.Trim())
                        .Where(url => !string.IsNullOrEmpty(url))
                        .ToList();
                }

                await _adminProductService.UpdateProductAsync(Product);
                TempData["Success"] = "Product updated successfully";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                
                // Reload categories on error
                var categories = await _adminProductService.GetCategoriesAsync();
                Categories = new SelectList(categories, "Id", "Name");
                return Page();
            }
        }
    }
} 