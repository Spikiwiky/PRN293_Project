using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

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
            return await _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Customer)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
        
        public async Task<List<OrderDetail>> GetAllOrderDetailsAsync()
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .ToListAsync();
        }
        
        public async Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }
        
        public async Task<List<CartDetail>> GetCartDetailsByCartIdAsync(int cartId)
        {
            return await _context.CartDetails
                .Include(cd => cd.Product)
                .Where(cd => cd.CartId == cartId)
                .ToListAsync();
        }
        
        public async Task<Order?> CreateOrderAsync(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch
            {
                return null;
            }
        }
        
        public async Task<List<OrderDetail>> CreateOrderDetailsAsync(List<OrderDetail> orderDetails)
        {
            try
            {
                _context.OrderDetails.AddRange(orderDetails);
                await _context.SaveChangesAsync();
                return orderDetails;
            }
            catch
            {
                return new List<OrderDetail>();
            }
        }

        public async Task<List<OrderStatus>> GetAllOrderStatusesAsync()
        {
            return await _context.OrderStatuses.ToListAsync();
        }

        public async Task<List<PaymentMethod>> GetAllPaymentMethodsAsync()
        {
            return await _context.PaymentMethods.ToListAsync();
        }
    }
} 