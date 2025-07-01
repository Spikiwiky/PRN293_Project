using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EcommerceDBContext _context;
        public OrderRepository(EcommerceDBContext context)
        {
            _context = context;
        }
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders.ToListAsync();
        }
        public async Task<List<OrderDetail>> GetAllOrderDetailsAsync()
        {
            return await _context.OrderDetails.ToListAsync();
        }
    }
} 