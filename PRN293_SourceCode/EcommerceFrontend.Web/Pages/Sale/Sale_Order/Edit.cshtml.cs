using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using EcommerceFrontend.Web.Models.DTOs;

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

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
      
            var response = await _httpClient.GetAsync($"api/saleorder/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var order = JsonSerializer.Deserialize<OrderResponseDto>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (order == null)
                {
                    ErrorMessage = "Không tìm thấy đơn hàng.";
 
                    OrderDto = new UpdateOrderDto
                    {
                        OrderDetails = new List<OrderDetailRequestDto>()
                    };
                    return Page();
                }
                 
                OrderDto = new UpdateOrderDto
                {
                    CustomerId = order.CustomerId,
                    PaymentMethodId = order.PaymentMethodId,
                    OrderDetails = order.OrderDetails.Select(d => new OrderDetailRequestDto
                    {
                        ProductId = d.ProductId,
                        VariantId = d.VariantId,
                        Quantity = d.Quantity
                    }).ToList()
                };

                return Page();
            }

            ErrorMessage = $"Không thể lấy dữ liệu đơn hàng. Lỗi: {response.ReasonPhrase}";
          
            OrderDto = new UpdateOrderDto
            {
                OrderDetails = new List<OrderDetailRequestDto>()
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Dữ liệu không hợp lệ.";
                return Page();
            }

            var json = JsonSerializer.Serialize(OrderDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

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
