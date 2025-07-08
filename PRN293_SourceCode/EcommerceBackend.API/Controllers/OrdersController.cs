using EcommerceBackend.BusinessObject.Services.OrderService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EcommerceBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
        [HttpGet("details")]
        public async Task<IActionResult> GetAllOrderDetails()
        {
            var details = await _orderService.GetAllOrderDetailsAsync();
            return Ok(details);
        }
    }
} 