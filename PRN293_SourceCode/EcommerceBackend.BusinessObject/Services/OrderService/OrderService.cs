
//using EcommerceBackend.BusinessObject.dtos;
//using EcommerceBackend.DataAccess.Abstract;
//using EcommerceBackend.DataAccess.Models;
//using System.Collections.Generic;
//using System.Linq;

//﻿using EcommerceBackend.BusinessObject.Abstract.OrderAbstract;
//using EcommerceBackend.BusinessObject.dtos.OrderDto;
//using EcommerceBackend.DataAccess.Abstract.OrderAbstract;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using System.Threading.Tasks;

//namespace EcommerceBackend.BusinessObject.Services.OrderService
//{

//    public class OrderService : IOrderService
//    {
//        private readonly IOrderRepository _orderRepository;
//        public OrderService(IOrderRepository orderRepository)
//        {
//            _orderRepository = orderRepository;
//        }
//        public async Task<List<OrderDto>> GetAllOrdersAsync()
//        {
//            var orders = await _orderRepository.GetAllOrdersAsync();
//            return orders.Select(o => new OrderDto
//            {
//                OrderId = o.OrderId,
//                CustomerId = o.CustomerId,
//                TotalQuantity = o.TotalQuantity,
//                AmountDue = o.AmountDue,
//                PaymentMethodId = o.PaymentMethodId,
//                OrderStatusId = o.OrderStatusId,
//                OrderNote = o.OrderNote
//            }).ToList();
//        }
//        public async Task<List<OrderDetailDto>> GetAllOrderDetailsAsync()
//        {
//            var details = await _orderRepository.GetAllOrderDetailsAsync();
//            return details.Select(d => new OrderDetailDto
//            {
//                OrderDetailId = d.OrderDetailId,
//                OrderId = d.OrderId,
//                ProductId = d.ProductId,
//                VariantId = d.VariantId,
//                ProductName = d.ProductName,
//                Quantity = d.Quantity,
//                Price = d.Price
//            }).ToList();
//        }
//    }
//} 

//    public class OrderService:IOrderService
//    {
//        private readonly IConfiguration _configuration;
//        private readonly IOrderRepository _orderRepository;

//        public OrderService(IOrderRepository OrderRepository, IConfiguration configuration)
//        {
//            _orderRepository = OrderRepository;
//            _configuration = configuration;
//        }


//        public async Task<OrderDto> GetOrderDetailsAsync(int orderId)
//        {
//            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
//            if (order == null)
//            {
//                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
//            }

//            var orderDto = new OrderDto
//            {
//                OrderId = order.OrderId,
//                CustomerId = order.CustomerId,
//                CustomerName = order.Customer?.UserName,
//                TotalQuantity = order.TotalQuantity,
//                AmountDue = order.AmountDue,
//                PaymentMethod = order.PaymentMethod?.PaymentMethodTittle,
//                OrderStatus = order.OrderStatus?.OrderStatusTittle,
//                OrderNote = order.OrderNote,
//                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
//                {
//                    OrderDetailId = od.OrderDetailId,
//                    ProductId = od.ProductId,
//                    VariantId = od.VariantId,
//                    ProductName = od.ProductName,
//                    Quantity = od.Quantity,
//                    Price = od.Price
//                }).ToList()
//            };

//            return orderDto;
//        }


//        public async Task IncreaseQuantityAsync(int orderId, int productId, string? variantId)
//        {
//            var detail = await _orderRepository.GetOrderDetailAsync(orderId, productId, variantId)
//                         ?? throw new Exception("Không tìm thấy chi tiết đơn hàng.");

//            detail.Quantity = (detail.Quantity ?? 0) + 1;

//            var order = await _orderRepository.GetOrderUpdateQuantityAsync(orderId)
//                         ?? throw new Exception("Không tìm thấy đơn hàng.");

//            order.TotalQuantity = order.OrderDetails.Sum(d => d.Quantity ?? 0);
//            order.AmountDue = order.OrderDetails.Sum(d => (d.Quantity ?? 0) * (d.Price ?? 0));

//            await _orderRepository.SaveChangesAsync();
//        }

//        public async Task DecreaseQuantityAsync(int orderId, int productId, string? variantId)
//        {
//            var detail = await _orderRepository.GetOrderDetailAsync(orderId, productId, variantId)
//                         ?? throw new Exception("Không tìm thấy chi tiết đơn hàng.");

//            detail.Quantity = (detail.Quantity ?? 0) - 1;

//            if (detail.Quantity <= 0)
//            {
//                _orderRepository.RemoveOrderDetail(detail);
//            }

//            var order = await _orderRepository.GetOrderUpdateQuantityAsync(orderId)
//                         ?? throw new Exception("Không tìm thấy đơn hàng.");

//            order.TotalQuantity = order.OrderDetails.Sum(d => d.Quantity ?? 0);
//            order.AmountDue = order.OrderDetails.Sum(d => (d.Quantity ?? 0) * (d.Price ?? 0));

//            await _orderRepository.SaveChangesAsync();
//        }

//    }
//}

