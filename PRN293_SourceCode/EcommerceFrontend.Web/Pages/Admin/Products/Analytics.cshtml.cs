using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceFrontend.Web.Pages.Admin.Products
{
    public class AnalyticsModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        public int TotalOrders { get; set; }
        public int TotalProductsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<string> ProductNames { get; set; } = new();
        public List<decimal> ProductRevenues { get; set; } = new();
        public AnalyticsModel(IProductService productService, IOrderService orderService)
        {
            _productService = productService;
            _orderService = orderService;
        }
        public async Task OnGetAsync()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var orderDetails = await _orderService.GetAllOrderDetailsAsync();
            var products = await _productService.GetAllProductsAsync();
            TotalOrders = orders.Count;
            TotalProductsSold = (int)orderDetails.Sum(od => od.Quantity);
            TotalRevenue = (decimal)orderDetails.Sum(od => od.Price * od.Quantity);
            ProductNames = products.Select(p => p.Name).ToList();
            ProductRevenues = new List<decimal>();
            foreach (var p in products)
            {
                var revenue = orderDetails.Where(od => od.ProductId == p.ProductId).Sum(od => od.Price * od.Quantity);
                ProductRevenues.Add((decimal)revenue);
            }
        }
    }
} 