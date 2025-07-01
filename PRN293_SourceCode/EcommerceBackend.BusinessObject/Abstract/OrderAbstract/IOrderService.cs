using EcommerceBackend.BusinessObject.dtos.OrderDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Abstract.OrderAbstract
{
    public interface IOrderService
    {
        Task<OrderDto> GetOrderDetailsAsync(int orderId);
        Task IncreaseQuantityAsync(int orderId, int productId, string? variantId);
        Task DecreaseQuantityAsync(int orderId, int productId, string? variantId);
    }
}
