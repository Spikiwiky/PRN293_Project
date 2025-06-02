using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Services.Admin;

namespace EcommerceFrontend.Web.Pages.Admin.Products
{
    public class CreateModel : PageModel
    {
        private readonly IAdminProductService _adminProductService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IAdminProductService adminProductService, ILogger<CreateModel> logger)
        {
            _adminProductService = adminProductService;
            _logger = logger;
        }

        [BindProperty]
        public AdminProductCreateDto Product { get; set; } = new();

        [BindProperty]
        public string? ImageUrlsInput { get; set; }

        public SelectList Categories { get; set; }

        public async Task OnGetAsync()
        {
            // Get categories from service
            var categories = await _adminProductService.GetCategoriesAsync();
            Categories = new SelectList(categories, "Id", "Name");
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

                // Generate a new variant ID if not provided
                if (string.IsNullOrEmpty(Product.VariantId))
                {
                    Product.VariantId = Guid.NewGuid().ToString();
                }

                await _adminProductService.CreateProductAsync(Product);
                TempData["Success"] = "Product created successfully";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                
                // Reload categories on error
                var categories = await _adminProductService.GetCategoriesAsync();
                Categories = new SelectList(categories, "Id", "Name");
                return Page();
            }
        }
    }
} 