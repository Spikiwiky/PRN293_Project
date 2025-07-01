using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Abstract.OrderAbstract
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<OrderDetail?> GetOrderDetailAsync(int orderId, int productId, string? variantId);
        Task SaveChangesAsync();
        void RemoveOrderDetail(OrderDetail orderDetail);
        Task<Order?> GetOrderUpdateQuantityAsync(int orderId);
    }
}
