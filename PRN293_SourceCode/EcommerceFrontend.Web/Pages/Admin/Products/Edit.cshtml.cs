using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceFrontend.Web.Pages.Admin.Products
{
    public class EditModel : PageModel
    {
        private readonly IProductService _productService;
        [BindProperty]
        public ProductDTO Product { get; set; } = new ProductDTO();
        [BindProperty]
        public string AttributeName { get; set; } = string.Empty;
        [BindProperty]
        public List<string> AttributeValues { get; set; } = new List<string>();
        public Dictionary<string, List<string>> Attributes { get; set; } = new();
        public string? Message { get; set; }

        public EditModel(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            Product = product;
            Attributes = await _productService.GetProductAttributesAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Loại bỏ lỗi validation của các trường thuộc tính
            ModelState.Remove(nameof(AttributeName));
            ModelState.Remove(nameof(AttributeValues));

            if (!ModelState.IsValid)
            {
                Attributes = await _productService.GetProductAttributesAsync(id);
                return Page();
            }
            var result = await _productService.UpdateProductAsync(id, Product);
            if (result)
            {
                // Chuyển hướng về trang Index sau khi cập nhật thành công
                return RedirectToPage("Index");
            }
            else
            {
                Message = "Cập nhật sản phẩm thất bại!";
            }
            Attributes = await _productService.GetProductAttributesAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAttributeAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(AttributeName) || AttributeValues == null || !AttributeValues.Any())
            {
                Message = "Tên thuộc tính và giá trị không được để trống.";
                Attributes = await _productService.GetProductAttributesAsync(id);
                return Page();
            }
            var result = await _productService.UpdateProductAttributeAsync(id, AttributeName, AttributeValues);
            if (result)
                Message = "Cập nhật thuộc tính thành công!";
            else
                Message = "Cập nhật thuộc tính thất bại!";
            Product = await _productService.GetProductByIdAsync(id);
            Attributes = await _productService.GetProductAttributesAsync(id);
            return Page();
        }
    }
} 