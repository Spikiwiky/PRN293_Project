using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Options;
using EcommerceFrontend.Web.Models.Sale;

namespace EcommerceFrontend.Web.Pages.Admin.Products
{
    public class EditModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ApiSettings _apiSettings;

        [BindProperty]
        public ProductDTO Product { get; set; } = new ProductDTO();
        [BindProperty]
        public string AttributeName { get; set; } = string.Empty;
        [BindProperty]
        public List<string> AttributeValues { get; set; } = new List<string>();
        public Dictionary<string, List<string>> Attributes { get; set; } = new();
        public string? Message { get; set; }
        public string ApiBaseUrl => _apiSettings.BaseUrl;

        public EditModel(IProductService productService, IOptions<ApiSettings> apiSettings)
        {
            _productService = productService;
            _apiSettings = apiSettings.Value ?? throw new ArgumentNullException(nameof(apiSettings), "ApiSettings is not configured.");
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
                // Gọi API để upload ảnh thay vì lưu file trực tiếp
                var imageUrl = await _productService.UploadProductImageAsync(id, imageFile);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    // Ảnh đã được upload thành công, URL đã được trả về từ API
                    // Không cần gọi AddProductImageAsync vì API đã tự động lưu vào DB
                }
                else
                {
                    Message = "Upload ảnh thất bại!";
                    Attributes = await _productService.GetProductAttributesAsync(id);
                    return Page();
                }
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