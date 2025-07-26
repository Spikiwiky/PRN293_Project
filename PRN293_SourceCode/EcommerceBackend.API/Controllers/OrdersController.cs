using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EcommerceBackend.BusinessObject.Services.OrderService;
using EcommerceBackend.BusinessObject.dtos.OrderDto;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace EcommerceBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // GET: api/Orders/admin - Get all orders (Admin only)
        [HttpGet("admin")]
      
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(new { success = true, data = orders });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return StatusCode(500, new { success = false, message = "Lỗi server" });
            }
        }

        // GET: api/Orders/user - Get orders for current user
        [HttpGet("user")]
        public async Task<IActionResult> GetUserOrders()
        {
            try
            {
                // Lấy userId từ cookie thay vì JWT claims
                var userIdCookie = Request.Cookies["UserId"];
                if (string.IsNullOrEmpty(userIdCookie) || !int.TryParse(userIdCookie, out int userId))
                {
                    return BadRequest(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(new { success = true, data = orders });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user orders");
                return StatusCode(500, new { success = false, message = "Lỗi server" });
            }
        }

        // GET: api/Orders/{id} - Get specific order
        [HttpGet("{id}")]
        
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Check if user is authorized to view this order
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                // Admin can view any order, users can only view their own orders
                if (!User.IsInRole("Admin") && order.CustomerId != userId)
                {
                    return Forbid();
                }

                return Ok(new { success = true, data = order });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", id);
                return StatusCode(500, new { success = false, message = "Lỗi server" });
            }
        }

        // GET: api/Orders/{id}/details - Get order details
        [HttpGet("{id}/details")]
     
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            try
            {
                var orderDetails = await _orderService.GetOrderDetailsByOrderIdAsync(id);
                return Ok(new { success = true, data = orderDetails });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order details for order {OrderId}", id);
                return StatusCode(500, new { success = false, message = "Lỗi server" });
            }
        }

        // POST: api/Orders/create - Create new order from cart
        [HttpPost("create")]
       
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            try
            {
                _logger.LogInformation("Creating order with payment method: {PaymentMethod}", request.PaymentMethod);

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                var result = await _orderService.CreateOrderFromCartAsync(userId, request.PaymentMethod, request.OrderNote, request.ShippingAddress, request.ProvinceId, request.ProvinceName, request.DistrictId, request.DistrictName, request.WardCode, request.WardName, request.Subtotal, request.ShippingFee, request.TotalAmount);

                if (result.Success)
                {
                    _logger.LogInformation("Order created successfully for user ID: {UserId}", userId);

                    var response = new
                    {
                        success = true,
                        message = result.Message,
                        orderId = result.OrderId,
                        paymentUrl = string.IsNullOrEmpty(result.PaymentUrl) ? null : result.PaymentUrl
                    };

                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning("Order creation failed for user ID: {UserId}, Message: {Message}", userId, result.Message);
                    return BadRequest(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, new { success = false, message = "Lỗi khi tạo đơn hàng" });
            }
        }

        // GET: api/Orders/details - Get all order details (Admin only)
        [HttpGet("details")]
        
        public async Task<IActionResult> GetAllOrderDetails()
        {
            try
            {
                var orderDetails = await _orderService.GetAllOrderDetailsAsync();
                return Ok(new { success = true, data = orderDetails });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all order details");
                return StatusCode(500, new { success = false, message = "Lỗi server" });
            }
        }
    }
} 