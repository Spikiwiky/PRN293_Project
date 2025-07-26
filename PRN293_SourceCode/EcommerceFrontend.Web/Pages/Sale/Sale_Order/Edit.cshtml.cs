using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using EcommerceFrontend.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Order
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        [BindProperty]
        public UpdateOrderDto OrderDto { get; set; } = new UpdateOrderDto
        {
            OrderDetails = new List<OrderDetailRequestDto>()
        };

        public List<SelectListItem> OrderStatuses { get; set; } = new()
        {
            new SelectListItem { Value = "1", Text = "Pending" },
            new SelectListItem { Value = "2", Text = "Shipping" },
            new SelectListItem { Value = "3", Text = "Delivered" },
            new SelectListItem { Value = "4", Text = "Completed" },
            new SelectListItem { Value = "5", Text = "Canceled" }
        };
        public List<SelectListItem> PaymentMethods { get; set; } = new()
{
    new SelectListItem { Value = "1", Text = "Credit Card" },
    new SelectListItem { Value = "2", Text = "Debit Card" },
    new SelectListItem { Value = "3", Text = "PayPal" },
    new SelectListItem { Value = "4", Text = "Bank Transfer" },
    new SelectListItem { Value = "5", Text = "Cash on Delivery" },
    new SelectListItem { Value = "6", Text = "Mobile Payment" },
    new SelectListItem { Value = "7", Text = "Gift Card" },
    new SelectListItem { Value = "8", Text = "Cryptocurrency" },
    new SelectListItem { Value = "9", Text = "Digital Wallet" },
    new SelectListItem { Value = "10", Text = "Check" }
};


        // Danh sách variants dùng chung (có thể cải tiến load riêng từng product)
        public List<SelectListItem> VariantOptions { get; set; } = new();

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/saleorder/{id}");
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = $"Không thể lấy dữ liệu đơn hàng. Lỗi: {response.ReasonPhrase}";
                return Page();
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var order = JsonSerializer.Deserialize<OrderResponseDto>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (order == null)
            {
                ErrorMessage = "Không tìm thấy đơn hàng.";
                return Page();
            }

            OrderDto = new UpdateOrderDto
            {
                CustomerId = order.CustomerId,
                PaymentMethodId = order.PaymentMethodId,
                OrderStatusId = order.OrderStatusId,
                OrderNote = order.OrderNote,
                ShippingAddress = order.ShippingAddress,
                OrderDetails = order.OrderDetails.Select(d => new OrderDetailRequestDto
                {
                    ProductId = d.ProductId,
                    VariantId = d.VariantId,
                    Quantity = d.Quantity
                }).ToList()
            };

            // Nếu có ProductId thì load variants
            if (OrderDto.OrderDetails.Any() && OrderDto.OrderDetails[0].ProductId.HasValue)
            {
                await LoadVariants(OrderDto.OrderDetails[0].ProductId.Value);
            }

            return Page();
        }

        private async Task LoadVariants(int productId)
        {
            var response = await _httpClient.GetAsync($"api/SaleProduct/products/{productId}/variants");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var variants = JsonSerializer.Deserialize<List<ProductVariantDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (variants != null)
                {
                    VariantOptions = variants.Select(v => new SelectListItem
                    {
                        Value = v.VariantId.ToString(),
                        Text = $"{v.Attributes} - {v.Variants}"
                    }).ToList();
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Dữ liệu không hợp lệ.";
                return Page();
            }

            var json = JsonSerializer.Serialize(OrderDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/saleorder/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Sale/Sale_Order/Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ErrorMessage = $"Lỗi cập nhật đơn hàng: {errorContent}";
            return Page();
        }
    }
}
