using EcommerceBackend.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Abstract
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<List<OrderDetail>> GetAllOrderDetailsAsync();
        Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<List<CartDetail>> GetCartDetailsByCartIdAsync(int cartId);
        Task<Order?> CreateOrderAsync(Order order);
        Task<List<OrderDetail>> CreateOrderDetailsAsync(List<OrderDetail> orderDetails);
        Task<List<OrderStatus>> GetAllOrderStatusesAsync();
        Task<List<PaymentMethod>> GetAllPaymentMethodsAsync();
    }
} 