using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using EcommerceFrontend.Web.Models.DTOs;
using System.Text.Json;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Order
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public string ErrorMessage { get; set; }

        [BindProperty]
        public CreateOrderDto OrderDto { get; set; } = new CreateOrderDto();

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public IActionResult OnGet()
        {
            OrderDto.OrderDetails = new List<OrderDetailRequestDto> { new OrderDetailRequestDto() };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (OrderDto == null || OrderDto.OrderDetails == null || !OrderDto.OrderDetails.Any())
            {
                ErrorMessage = "Dữ liệu đơn hàng không hợp lệ hoặc không có sản phẩm.";
                return Page();
            }

            // Validation cơ bản trước khi gửi
            if (OrderDto.CustomerId <= 0)
            {
                ErrorMessage = "Customer ID phải lớn hơn 0.";
                return Page();
            }
            if (OrderDto.PaymentMethodId <= 0)
            {
                ErrorMessage = "Payment Method ID phải lớn hơn 0.";
                return Page();
            }
            foreach (var detail in OrderDto.OrderDetails)
            {
                if (!detail.ProductId.HasValue || detail.ProductId <= 0)
                {
                    ErrorMessage = "Product ID phải lớn hơn 0.";
                    return Page();
                }
                if (detail.Quantity <= 0)
                {
                    ErrorMessage = "Quantity phải lớn hơn 0.";
                    return Page();
                }
            }

            var json = JsonSerializer.Serialize(OrderDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/saleorder", content);

            if (response.IsSuccessStatusCode)
            {
                var contentString = await response.Content.ReadAsStringAsync();
                var createdOrder = JsonSerializer.Deserialize<OrderResponseDto>(contentString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (createdOrder != null)
                {
                    return RedirectToPage("/Sale/Sale_Order/Index");
                }
                ErrorMessage = "Không thể đọc dữ liệu đơn hàng đã tạo.";
                return Page();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ErrorMessage = $"Lỗi khi tạo đơn hàng: {errorContent}";
            }
            else
            {
                ErrorMessage = $"Lỗi khi tạo đơn hàng: {response.ReasonPhrase}";
            }
            return Page();
        }
    }
}