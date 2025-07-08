using EcommerceBackend.BusinessObject.dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.OrderService
{
    public interface IOrderService
    {
        //Task<List<OrderDto>> GetAllOrdersAsync();
        Task<List<OrderDetailDto>> GetAllOrderDetailsAsync();
    }
} 