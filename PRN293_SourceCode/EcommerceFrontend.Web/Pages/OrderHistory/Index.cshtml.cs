using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services.Order;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace EcommerceFrontend.Web.Pages.OrderHistory
{
    public class IndexModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IOrderService orderService, ILogger<IndexModel> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public IEnumerable<OrderDTO> Orders { get; set; } = new List<OrderDTO>();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Lấy userId từ cookie thay vì JWT claims
                var userIdCookie = Request.Cookies["UserId"];
                _logger.LogInformation("Cookie UserId value: {UserIdCookie}", userIdCookie);
                
                if (string.IsNullOrEmpty(userIdCookie))
                {
                    _logger.LogWarning("User ID cookie is null or empty");
                    ErrorMessage = "Vui lòng đăng nhập để xem lịch sử đơn hàng";
                    Orders = new List<OrderDTO>();
                    return Page();
                }
                
                if (!int.TryParse(userIdCookie, out int userId))
                {
                    _logger.LogWarning("User ID cookie value '{UserIdCookie}' cannot be parsed as integer", userIdCookie);
                    ErrorMessage = "Thông tin người dùng không hợp lệ";
                    Orders = new List<OrderDTO>();
                    return Page();
                }

                _logger.LogInformation("Successfully parsed userId: {UserId}", userId);
                Orders = await _orderService.GetOrdersByUserIdAsync(userId);
                _logger.LogInformation("Retrieved {OrderCount} orders for user {UserId}", Orders?.Count() ?? 0, userId);
                
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnGetAsync for OrderHistory");
                ErrorMessage = "Có lỗi xảy ra khi tải lịch sử đơn hàng. Vui lòng thử lại.";
                Orders = new List<OrderDTO>();
                return Page();
            }
        }
    }
} 