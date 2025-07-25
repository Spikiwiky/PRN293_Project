using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using EcommerceFrontend.Web.Models.DTOs;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Order
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<OrderResponseDto> Orders { get; set; } = new List<OrderResponseDto>();
        public string ErrorMessage { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/saleorder");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Orders = JsonSerializer.Deserialize<List<OrderResponseDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderResponseDto>();
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
                var response = await _httpClient.GetAsync($"api/saleorder/{id}/details");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"JSON response for id {id}: {content}");
                var orderDetails = JsonSerializer.Deserialize<List<OrderDetailResponseDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
                ErrorMessage = $"Lỗi khi chuyển đổi JSON thành OrderDetailResponseDto: {ex.Message}. Vui lòng kiểm tra cấu trúc dữ liệu.";
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
                var response = await _httpClient.DeleteAsync($"api/saleorder/{id}");
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
    }
}