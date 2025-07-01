using EcommerceFrontend.Web.Models.Order;
using EcommerceFrontend.Web.Services.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages.OrderPage
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

        [BindProperty(SupportsGet = true)]
        public int OrderId { get; set; }

        public OrderDto? Order { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (OrderId <= 0)
            {
                _logger.LogWarning("Invalid OrderId provided.");
                return RedirectToPage("/Error");
            }

            Order = await _orderService.GetOrderByIdAsync(OrderId);

            if (Order == null)
            {
                _logger.LogWarning("Order not found: {OrderId}", OrderId);
                return RedirectToPage("/Error");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostIncreaseAsync(int orderId, int productId, string? variantId)
        {
            var success = await _orderService.IncreaseQuantityAsync(orderId, productId, variantId);
            if (!success)
            {
                _logger.LogError("Increase quantity failed. OrderId: {OrderId}, VariantId: {VariantId}", orderId, variantId);
            }

            return RedirectToPage(new { OrderId = orderId });
        }

        public async Task<IActionResult> OnPostDecreaseAsync(int orderId, int productId, string? variantId)
        {
            var success = await _orderService.DecreaseQuantityAsync(orderId, productId, variantId);
            if (!success)
            {
                _logger.LogError("Decrease quantity failed. OrderId: {OrderId}, VariantId: {VariantId}", orderId, variantId);
            }

            return RedirectToPage(new { OrderId = orderId });
        }
    }
}
