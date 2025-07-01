using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;

namespace EcommerceFrontend.Web.Pages.Admin.Products
{
    public class VariantsModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ILogger<VariantsModel> _logger;

        public VariantsModel(IProductService productService, ILogger<VariantsModel> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } // ProductId

        public ProductDTO Product { get; set; }
        public List<ProductVariantDTO> Variants { get; set; } = new();
        [BindProperty]
        public ProductVariantDTO NewVariant { get; set; } = new();
        [BindProperty]
        public int? EditVariantId { get; set; }
        [BindProperty]
        public ProductVariantDTO EditVariant { get; set; } = new();
        [TempData]
        public string? StatusMessage { get; set; }
        [BindProperty]
        public string NewVariantVariantsJson { get; set; } = string.Empty;
        [BindProperty]
        public string NewVariantAttributesJson { get; set; } = string.Empty;
        public Dictionary<string, List<string>> AttributeOptions { get; set; } = new Dictionary<string, List<string>>();

        private async Task EnsureProductAndAttributesAsync()
        {
            if (Product == null)
            {
                Product = await _productService.GetProductByIdAsync(Id);
            }
            if (AttributeOptions == null || AttributeOptions.Count == 0)
            {
                AttributeOptions = new Dictionary<string, List<string>>();
                if (Product != null && !string.IsNullOrEmpty(Product.AvailableAttributes))
                {
                    try
                    {
                        AttributeOptions = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(Product.AvailableAttributes) ?? new();
                    }
                    catch { AttributeOptions = new(); }
                }
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation($"[OnGetAsync] Id = {Id}");
            await EnsureProductAndAttributesAsync();
            Variants = Product.Variants;
            return Page();
        }

        public async Task<IActionResult> OnPostAddVariantAsync()
        {
            await EnsureProductAndAttributesAsync();
            var selectedAttrs = Request.Form["SelectedAttributes"].ToList();
            var attrDict = new Dictionary<string, string>();
            foreach (var attr in selectedAttrs)
            {
                var value = Request.Form[$"NewVariant_{attr}"];
                if (!string.IsNullOrEmpty(value))
                    attrDict[attr] = value.ToString();
            }
            // Lấy price và stock (bắt buộc)
            var priceStr = Request.Form["NewVariant_price"];
            var stockStr = Request.Form["NewVariant_stock"];
            if (string.IsNullOrEmpty(priceStr) || !decimal.TryParse(priceStr, out var price) ||
                string.IsNullOrEmpty(stockStr) || !int.TryParse(stockStr, out var stock))
            {
                StatusMessage = "Vui lòng nhập price và stock.";
                await OnGetAsync();
                return Page();
            }

            // Tạo object mới cho giá trị biến thể
            var newValue = new Dictionary<string, object>(attrDict.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value));
            newValue["price"] = price;
            newValue["stock"] = stock;

            if (Product.Variants != null && Product.Variants.Count > 0)
            {
                // Luôn chỉ update vào variant đầu tiên
                var targetVariant = Product.Variants[0];
                var variantsList = targetVariant.Variants ?? new List<Dictionary<string, object>>();
                // Kiểm tra trùng giá trị
                bool isValueDuplicate = variantsList.Any(v =>
                    v.Keys.All(k => newValue.ContainsKey(k) && v[k]?.ToString() == newValue[k]?.ToString()) &&
                    newValue.Keys.All(k => v.ContainsKey(k) && v[k]?.ToString() == newValue[k]?.ToString())
                );
                if (isValueDuplicate)
                {
                    StatusMessage = "Giá trị biến thể này đã tồn tại trong danh sách.";
                    await OnGetAsync();
                    return Page();
                }
                variantsList.Add(newValue);
                targetVariant.Variants = variantsList;
                var result = await _productService.UpdateProductVariantAsync(targetVariant);
                StatusMessage = result ? "Thêm giá trị biến thể thành công." : "Thêm giá trị biến thể thất bại.";
            }
            else
            {
                // Chưa có variant nào, tạo mới 1 dòng duy nhất
                var newVariant = new ProductVariantDTO
                {
                    ProductId = Id,
                    Attributes = System.Text.Json.JsonSerializer.Serialize(attrDict),
                    Variants = new List<Dictionary<string, object>> { newValue }
                };
                var result = await _productService.AddProductVariantAsync(newVariant);
                StatusMessage = result ? "Thêm giá trị biến thể thành công." : "Thêm giá trị biến thể thất bại.";
            }
            return RedirectToPage(new { id = Id });
        }

        public async Task<IActionResult> OnPostEditVariantAsync()
        {
            if (!ModelState.IsValid || EditVariantId == null)
            {
                StatusMessage = "Dữ liệu không hợp lệ.";
                return await OnGetAsync();
            }
            EditVariant.ProductId = Id;
            EditVariant.VariantId = EditVariantId;
            var result = await _productService.UpdateProductVariantAsync(EditVariant);
            StatusMessage = result ? "Cập nhật biến thể thành công." : "Cập nhật biến thể thất bại.";
            return RedirectToPage(new { id = Id });
        }

        public async Task<IActionResult> OnPostDeleteVariantAsync(int variantId)
        {
            var result = await _productService.DeleteProductVariantAsync(variantId);
            StatusMessage = result ? "Xóa biến thể thành công." : "Xóa biến thể thất bại.";
            return RedirectToPage(new { id = Id });
        }

        public async Task<IActionResult> OnPostDeleteVariantValueAsync(int variantId, int valueIndex)
        {
            _logger.LogInformation($"[DEBUG] DeleteVariantValue: variantId={variantId}, valueIndex={valueIndex}");
            var result = await _productService.DeleteVariantValueAsync(variantId, valueIndex);
            StatusMessage = result ? "Xóa giá trị biến thể thành công." : "Xóa giá trị biến thể thất bại.";
            return RedirectToPage(new { id = Id });
        }
    }
} 