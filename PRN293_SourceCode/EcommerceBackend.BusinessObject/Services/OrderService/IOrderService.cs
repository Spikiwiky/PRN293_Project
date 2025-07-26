using EcommerceBackend.BusinessObject.dtos.OrderDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.OrderService
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<List<OrderDetailDto>> GetAllOrderDetailsAsync();
        Task<List<OrderDetailDto>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<CreateOrderResultDto> CreateOrderFromCartAsync(int userId, string paymentMethod, string? orderNote, string? shippingAddress, int? provinceId = null, string? provinceName = null, int? districtId = null, string? districtName = null, string? wardCode = null, string? wardName = null, decimal subtotal = 0, decimal shippingFee = 0, decimal totalAmount = 0);
    }
} 