using EcommerceFrontend.Web.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceFrontend.Web.Services.Order
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(int userId);
        Task<OrderDTO?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDetailDTO>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<CreateOrderResultDTO> CreateOrderFromCartAsync(int userId, string paymentMethod, string? orderNote, string? shippingAddress, int? provinceId = null, string? provinceName = null, int? districtId = null, string? districtName = null, string? wardCode = null, string? wardName = null, decimal subtotal = 0, decimal shippingFee = 0, decimal totalAmount = 0);
    }
} 