using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceFrontend.Web.Models.Sale;
using EcommerceFrontend.Web.Services.Sale;

namespace EcommerceFrontend.Web.Pages.Sale.Products2
{
    public class EditModel : PageModel
    {
        private readonly ISaleProductService _saleProductService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ISaleProductService saleProductService, ILogger<EditModel> logger)
        {
            _saleProductService = saleProductService;
            _logger = logger;
        }

        [BindProperty]
        public SaleProductUpdateDto Product { get; set; } = new();

        [BindProperty]
        public string? ImageUrlsInput { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var product = await _saleProductService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return RedirectToPage("./Index");
                }

                // Get categories for dropdown
                var categories = await _saleProductService.GetCategoriesAsync();
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                // Map product data to form
                Product = new SaleProductUpdateDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    ProductCategoryId = product.ProductCategoryId,
                    Status = product.Status,
                    Variants = product.Variants,
                    ImageUrls = product.ImageUrls,
                    UpdatedBy = "admin" // Set a default value or get from user session
                };

                // Set image URLs input
                ImageUrlsInput = string.Join(Environment.NewLine, product.ImageUrls);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product for edit");
                TempData["Error"] = "Failed to load product. Please try again later.";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var categories = await _saleProductService.GetCategoriesAsync();
                    Categories = categories.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
                    return Page();
                }

                // Handle image URLs
                if (!string.IsNullOrEmpty(ImageUrlsInput))
                {
                    Product.ImageUrls = ImageUrlsInput
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(url => url.Trim())
                        .ToList();
                }

                // Set UpdatedBy if not already set
                if (string.IsNullOrEmpty(Product.UpdatedBy))
                {
                    Product.UpdatedBy = "admin"; // Set a default value or get from user session
                }

                await _saleProductService.UpdateProductAsync(Product);
                TempData["Success"] = "Product updated successfully";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                ModelState.AddModelError("", "Error updating product. Please try again.");
                var categories = await _saleProductService.GetCategoriesAsync();
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
                return Page();
            }
        }
    }
} 