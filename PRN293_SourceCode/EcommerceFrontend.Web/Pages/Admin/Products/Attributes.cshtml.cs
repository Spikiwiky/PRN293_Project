using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceFrontend.Web.Pages.Admin.Products
{
    public class AttributesModel : PageModel
    {
        private readonly IProductService _productService;
        public ProductDTO? Product { get; set; }
        public Dictionary<string, List<string>> Attributes { get; set; } = new();
        [BindProperty]
        public string AttributeName { get; set; } = string.Empty;
        [BindProperty]
        public List<string> AttributeValues { get; set; } = new List<string>();
        public string? Message { get; set; }

        public AttributesModel(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _productService.GetProductByIdAsync(id);
            if (Product == null) return NotFound();
            Attributes = await _productService.GetProductAttributesAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAddOrUpdateAsync(int id)
        {
            // Luôn tách giá trị thuộc tính từ textarea thành list
            var values = Request.Form["AttributeValues"].ToString();
            AttributeValues = values
                .Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            if (string.IsNullOrWhiteSpace(AttributeName) || AttributeValues == null || !AttributeValues.Any())
            {
                Message = "Tên thuộc tính và giá trị không được để trống.";
                Attributes = await _productService.GetProductAttributesAsync(id);
                Product = await _productService.GetProductByIdAsync(id);
                return Page();
            }

            var currentAttributes = await _productService.GetProductAttributesAsync(id);
            bool result;
            if (currentAttributes.ContainsKey(AttributeName))
            {
                result = await _productService.UpdateProductAttributeAsync(id, AttributeName, AttributeValues);
            }
            else
            {
                result = await _productService.AddProductAttributeAsync(id, AttributeName, AttributeValues);
            }
            Message = result ? "Cập nhật thuộc tính thành công!" : "Cập nhật thuộc tính thất bại!";
            Attributes = await _productService.GetProductAttributesAsync(id);
            Product = await _productService.GetProductByIdAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id, string attributeName)
        {
            var result = await _productService.DeleteProductAttributeAsync(id, attributeName);
            Message = result ? "Xóa thuộc tính thành công!" : "Xóa thuộc tính thất bại!";
            Attributes = await _productService.GetProductAttributesAsync(id);
            Product = await _productService.GetProductByIdAsync(id);
            return Page();
        }
    }
} 