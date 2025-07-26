using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using EcommerceFrontend.Web.Models.DTOs;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Order
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<OrderResponseDto> Orders { get; set; } = new();
        public string ErrorMessage { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/SaleOrder");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                Orders = JsonSerializer.Deserialize<List<OrderResponseDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderResponseDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi khi lấy danh sách đơn hàng: {ex.Message}";
            }
            return Page();
        }

        public async Task<IActionResult> OnGetDetailsAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/SaleOrder/{id}/details");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var orderDetails = JsonSerializer.Deserialize<List<OrderDetailResponseDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (orderDetails == null || !orderDetails.Any())
                {
                    ErrorMessage = $"Đơn hàng với ID {id} không có chi tiết.";
                    return Page();
                }

                TempData["OrderDetails"] = JsonSerializer.Serialize(orderDetails);
                return RedirectToPage("/Sale/Sale_Order/Details", new { id });
            }
            catch (JsonException ex)
            {
                ErrorMessage = $"Lỗi khi chuyển đổi JSON thành OrderDetailResponseDto: {ex.Message}";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi khi lấy chi tiết đơn hàng: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/SaleOrder/{id}");
                response.EnsureSuccessStatusCode();
                return RedirectToPage("/Sale/Sale_Order/Index");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ErrorMessage = $"Đơn hàng với ID {id} không tồn tại.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi khi xóa đơn hàng với ID {id}: {ex.Message}";
                return Page();
            }
        }
         
        public static string GetStatusName(int? statusId)
        {
            return statusId switch
            {
                1 => "Pending",
                2 => "Shipping",
                3 => "Delivered",
                4 => "Completed",
                5 => "Canceled",
                _ => "Unknown"
            };
        }
    }
}
