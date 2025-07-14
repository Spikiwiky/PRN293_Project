using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;

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

        public async Task<IActionResult> OnPostAsync(int id, IFormFile? imageFile)
        {
            // Loại bỏ lỗi validation của các trường thuộc tính
            ModelState.Remove(nameof(AttributeName));

            if (!ModelState.IsValid)
            {
                Attributes = await _productService.GetProductAttributesAsync(id);
                return Page();
            }

            // Xử lý upload ảnh nếu có file
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "images", "products");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var fileName = $"product_{id}_{DateTime.Now.Ticks}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                var imageUrl = $"/images/products/{fileName}";
                // Gọi service để thêm ảnh vào DB
                await _productService.AddProductImageAsync(id, imageUrl);
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
    }
} 