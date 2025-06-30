using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages.Sale.Products2;

public class VariantsModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ILogger<VariantsModel> _logger;

    public VariantsModel(IProductService productService, ILogger<VariantsModel> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    //[BindProperty(SupportsGet = true)]
    //public int ProductId { get; set; }

    //public ProductDTO? Product { get; set; }

    //[BindProperty]
    //public AddVariantDTO NewVariant { get; set; } = new();

    //public string? ErrorMessage { get; set; }
    //public string? SuccessMessage { get; set; }

    //public async Task<IActionResult> OnGetAsync()
    //{
    //    try
    //    {
    //        Product = await _productService.GetProductByIdAsync(ProductId);

    //        if (Product == null)
    //        {
    //            return NotFound($"Product with ID {ProductId} not found.");
    //        }

    //        return Page();
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error loading product variants");
    //        ErrorMessage = "Failed to load product variants. Please try again.";
    //        return Page();
    //    }
    //}

    //public async Task<IActionResult> OnPostAsync()
    //{
    //    try
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            Product = await _productService.GetProductByIdAsync(ProductId);
    //            return Page();
    //        }

    //        NewVariant.ProductId = ProductId;
    //        var variant = await _productService.AddVariantAsync(NewVariant);
            
    //        SuccessMessage = $"Successfully added new variant with ID {variant.VariantId}";
    //        return RedirectToPage(new { ProductId });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error adding variant");
    //        ErrorMessage = "Failed to add variant. Please try again.";
    //        Product = await _productService.GetProductByIdAsync(ProductId);
    //        return Page();
    //    }
    //}
} 