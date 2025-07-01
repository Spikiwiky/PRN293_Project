using EcommerceBackend.DataAccess.Abstract.OrderAbstract;
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository.OrderRepository
{
    public  class OrderRepository:IOrderRepository
    {
        private readonly EcommerceDBContext _context;
        public OrderRepository(EcommerceDBContext context)
        {
            _context = context;
        }


        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Customer)
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.OrderStatusId == 1);
        }

        public async Task<OrderDetail?> GetOrderDetailAsync(int orderId, int productId, string? variantId)
        {
            return await _context.OrderDetails
                .FirstOrDefaultAsync(od => od.OrderId == orderId
                                        && od.ProductId == productId
                                        && od.VariantId == variantId);
        }
        public async Task<Order?> GetOrderUpdateQuantityAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
        public void RemoveOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Remove(orderDetail);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
