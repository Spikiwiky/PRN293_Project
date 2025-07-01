using EcommerceBackend.BusinessObject.dtos;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                TotalQuantity = o.TotalQuantity,
                AmountDue = o.AmountDue,
                PaymentMethodId = o.PaymentMethodId,
                OrderStatusId = o.OrderStatusId,
                OrderNote = o.OrderNote
            }).ToList();
        }
        public async Task<List<OrderDetailDto>> GetAllOrderDetailsAsync()
        {
            var details = await _orderRepository.GetAllOrderDetailsAsync();
            return details.Select(d => new OrderDetailDto
            {
                OrderDetailId = d.OrderDetailId,
                OrderId = d.OrderId,
                ProductId = d.ProductId,
                VariantId = d.VariantId,
                ProductName = d.ProductName,
                Quantity = d.Quantity,
                Price = d.Price
            }).ToList();
        }
    }
} 