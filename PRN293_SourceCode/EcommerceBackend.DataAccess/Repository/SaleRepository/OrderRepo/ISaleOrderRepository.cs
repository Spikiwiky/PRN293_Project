using EcommerceBackend.DataAccess.Models;

namespace EcommerceBackend.DataAccess.Repository.SaleRepository.OrderRepo
{
    public interface ISaleOrderRepository
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task SaveChangesAsync();
        Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
    }
}
