using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using EcommerceFrontend.Web.Models.DTOs;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Order
{
    public class DetailsModel : PageModel
    {
        public List<OrderDetailResponseDto> OrderDetails { get; set; } = new List<OrderDetailResponseDto>();
        public string ErrorMessage { get; set; }

        // Thêm thuộc tính Order Info
        public int? CustomerId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? OrderStatusId { get; set; }
        public string? OrderNote { get; set; }
        public string? ShippingAddress { get; set; }

        public string PaymentMethodName => PaymentMethods.FirstOrDefault(x => x.Value == PaymentMethodId?.ToString())?.Text ?? "N/A";
        public string OrderStatusName => OrderStatuses.FirstOrDefault(x => x.Value == OrderStatusId?.ToString())?.Text ?? "N/A";

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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var orderDetailsJson = TempData["OrderDetails"] as string;
            var orderInfoJson = TempData["OrderInfo"] as string;

            if (string.IsNullOrEmpty(orderDetailsJson))
            {
                ErrorMessage = $"Không thể lấy chi tiết đơn hàng với ID {id}.";
                return Page();
            }

            try
            {
                OrderDetails = JsonSerializer.Deserialize<List<OrderDetailResponseDto>>(orderDetailsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderDetailResponseDto>();

                if (!OrderDetails.Any())
                    ErrorMessage = $"Đơn hàng với ID {id} không có chi tiết.";

                // Lấy thông tin chung đơn hàng (nếu có truyền)
                if (!string.IsNullOrEmpty(orderInfoJson))
                {
                    var order = JsonSerializer.Deserialize<OrderResponseDto>(orderInfoJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (order != null)
                    {
                        CustomerId = order.CustomerId;
                        PaymentMethodId = order.PaymentMethodId;
                        OrderStatusId = order.OrderStatusId;
                        OrderNote = order.OrderNote;
                        ShippingAddress = order.ShippingAddress;
                    }
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
