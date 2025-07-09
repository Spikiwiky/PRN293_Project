using EcommerceBackend.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Abstract
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<OrderDetail>> GetAllOrderDetailsAsync();
    }
} 