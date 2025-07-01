using EcommerceBackend.BusinessObject.Abstract.OrderAbstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceBackend.API.Controllers.OrderController
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        //[HttpGet("{orderId}")]
        //public async Task<IActionResult> GetOrderDetails(int orderId)
        //{
        //    if (orderId <= 0)
        //    {
        //        return BadRequest("Invalid order ID.");
        //    }

        //    try
        //    {
        //        //var order = await _orderService.GetOrderDetailsAsync(orderId);

        //        // (Optional) Map entity to DTO if you don't want to expose full EF models
        //        return Ok(order);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "An unexpected error occurred.");
        //    }
        //}

        [HttpPost("{orderId}/increase")]
        public async Task<IActionResult> IncreaseQuantity(
    int orderId, [FromQuery] int productId, [FromQuery] string? variantId)
        {
            try
            {
                await _orderService.IncreaseQuantityAsync(orderId, productId, variantId);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{orderId}/decrease")]
        public async Task<IActionResult> DecreaseQuantity(
            int orderId, [FromQuery] int productId, [FromQuery] string? variantId)
        {
            try
            {
                await _orderService.DecreaseQuantityAsync(orderId, productId, variantId);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
