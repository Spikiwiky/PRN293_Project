using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Services.Admin;

namespace EcommerceFrontend.Web.Pages.Admin.Products
{
    public class CreateModel : PageModel
    {
        private readonly IAdminProductService _productService;
        private readonly ILogger<CreateModel> _logger;

        [BindProperty]
        public AdminProductCreateDto Product { get; set; } = new();

        public List<SelectListItem> Categories { get; set; } = new();

        public CreateModel(IAdminProductService productService, ILogger<CreateModel> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var categories = await _productService.GetCategoriesAsync();
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
                TempData["Error"] = "Failed to load categories. Please try again later.";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var categories = await _productService.GetCategoriesAsync();
                    Categories = categories.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
                    return Page();
                }

                // Set CreatedBy field
                Product.CreatedBy = "admin"; // You might want to get this from user session/claims

                // Ensure at least one variant exists
                if (Product.Variants == null || !Product.Variants.Any())
                {
                    ModelState.AddModelError("", "At least one product variant is required.");
                    return Page();
                }

                // Validate variants
                foreach (var variant in Product.Variants)
                {
                    if (string.IsNullOrEmpty(variant.Size) || string.IsNullOrEmpty(variant.Color))
                    {
                        ModelState.AddModelError("", "Size and Color are required for all variants.");
                        return Page();
                    }

                    if (variant.Price <= 0)
                    {
                        ModelState.AddModelError("", "Price must be greater than 0 for all variants.");
                        return Page();
                    }

                    if (variant.StockQuantity < 0)
                    {
                        ModelState.AddModelError("", "Stock quantity cannot be negative.");
                        return Page();
                    }

                    // Set variant ID and Categories
                    variant.VariantId = $"{variant.Size}-{variant.Color}";
                    variant.Categories = "Default"; // Set a default category for now
                    variant.CreatedBy = "admin"; // Set the same creator as the product
                }

                // Create the product
                var createdProduct = await _productService.CreateProductAsync(Product);
                TempData["Success"] = "Product created successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                ModelState.AddModelError("", "Error creating product. Please try again.");
                var categories = await _productService.GetCategoriesAsync();
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