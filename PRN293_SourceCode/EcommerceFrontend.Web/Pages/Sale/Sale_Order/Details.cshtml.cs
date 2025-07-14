using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using EcommerceFrontend.Web.Models.DTOs;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Order
{
    public class DetailsModel : PageModel
    {
        public List<OrderDetailResponseDto> OrderDetails { get; set; } = new List<OrderDetailResponseDto>();
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Lấy dữ liệu từ TempData
            var orderDetailsJson = TempData["OrderDetails"] as string;
            if (string.IsNullOrEmpty(orderDetailsJson))
            {
                ErrorMessage = $"Không thể lấy chi tiết đơn hàng với ID {id}.";
                return Page();
            }

            try
            {
                OrderDetails = JsonSerializer.Deserialize<List<OrderDetailResponseDto>>(orderDetailsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderDetailResponseDto>();
                if (!OrderDetails.Any())
                {
                    ErrorMessage = $"Đơn hàng với ID {id} không có chi tiết.";
                }
            }
            catch (JsonException ex)
            {
                ErrorMessage = $"Lỗi khi chuyển đổi dữ liệu chi tiết: {ex.Message}";
                return Page();
            }

            return Page();
        }
    }
}