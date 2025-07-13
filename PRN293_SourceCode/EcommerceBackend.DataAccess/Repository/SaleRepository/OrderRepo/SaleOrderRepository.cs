using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBackend.DataAccess.Repository.SaleRepository.OrderRepo
{
    public class SaleOrderRepository : ISaleOrderRepository
    {
        private readonly EcommerceDBContext _context;

        public SaleOrderRepository(EcommerceDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Customer)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await SaveChangesAsync();
        }

        //public async Task DeleteOrderAsync(int id)
        //{
        //    var order = await GetOrderByIdAsync(id);
        //    if (order != null)
        //    {
        //        _context.Orders.Remove(order);
        //        await SaveChangesAsync();
        //    }
        //}

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
