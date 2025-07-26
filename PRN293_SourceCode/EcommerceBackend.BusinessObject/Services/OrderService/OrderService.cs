using EcommerceBackend.BusinessObject.dtos.OrderDto;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;

namespace EcommerceBackend.BusinessObject.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _logger = logger;
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return orders.Select(o => MapToOrderDto(o)).ToList();
        }

        public async Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return orders.Select(o => MapToOrderDto(o)).ToList();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            return order != null ? MapToOrderDto(order) : null;
        }

        public async Task<List<OrderDetailDto>> GetAllOrderDetailsAsync()
        {
            var orderDetails = await _orderRepository.GetAllOrderDetailsAsync();
            return orderDetails.Select(od => MapToOrderDetailDto(od)).ToList();
        }

        public async Task<List<OrderDetailDto>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            var orderDetails = await _orderRepository.GetOrderDetailsByOrderIdAsync(orderId);
            return orderDetails.Select(od => MapToOrderDetailDto(od)).ToList();
        }

        public async Task<CreateOrderResultDto> CreateOrderFromCartAsync(int userId, string paymentMethod, string? orderNote, string? shippingAddress, int? provinceId = null, string? provinceName = null, int? districtId = null, string? districtName = null, string? wardCode = null, string? wardName = null, decimal subtotal = 0, decimal shippingFee = 0, decimal totalAmount = 0)
        {
            try
            {
                _logger.LogInformation("Creating order from cart for user ID: {UserId}", userId);

                // Get user's cart
                var cart = await _cartRepository.GetCartByCustomerIdAsync(userId);
                if (cart == null)
                {
                    return new CreateOrderResultDto
                    {
                        Success = false,
                        Message = "Giỏ hàng không tồn tại"
                    };
                }

                // Get cart details (products in cart)
                var cartDetails = await _orderRepository.GetCartDetailsByCartIdAsync(cart.CartId);
                if (!cartDetails.Any())
                {
                    return new CreateOrderResultDto
                    {
                        Success = false,
                        Message = "Giỏ hàng trống"
                    };
                }

                // Calculate total from cart or use provided values
                var calculatedSubtotal = cartDetails.Sum(item => (item.Price ?? 0) * (item.Quantity ?? 0));
                var finalSubtotal = subtotal > 0 ? subtotal : calculatedSubtotal;
                var finalShippingFee = shippingFee;
                var finalTotalAmount = totalAmount > 0 ? totalAmount : (finalSubtotal + finalShippingFee);
                var totalQuantity = cartDetails.Sum(item => item.Quantity ?? 0);

                // Build complete shipping address
                var completeShippingAddress = shippingAddress;
                if (!string.IsNullOrEmpty(provinceName) && !string.IsNullOrEmpty(districtName) && !string.IsNullOrEmpty(wardName))
                {
                    completeShippingAddress = $"{shippingAddress}, {wardName}, {districtName}, {provinceName}".Trim();
                }

                // Create order
                var order = new Order
                {
                    CustomerId = userId,
                    TotalQuantity = totalQuantity,
                    AmountDue = finalTotalAmount,
                    OrderNote = orderNote,
                    ShippingAddress = completeShippingAddress,
                    PaymentMethodId = GetPaymentMethodId(paymentMethod),
                    OrderStatusId = 1, // Pending status
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Save order to database
                var savedOrder = await _orderRepository.CreateOrderAsync(order);
                if (savedOrder == null)
                {
                    return new CreateOrderResultDto
                    {
                        Success = false,
                        Message = "Không thể tạo đơn hàng"
                    };
                }

                // Create order details from cart details
                var orderDetails = cartDetails.Select(item => new OrderDetail
                {
                    OrderId = savedOrder.OrderId,
                    ProductId = item.ProductId ?? 0,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity ?? 0,
                    Price = item.Price ?? 0,
                    VariantId = item.VariantId,
                    VariantAttributes = item.VariantAttributes
                }).ToList();

                // Save order details
                await _orderRepository.CreateOrderDetailsAsync(orderDetails);

                // Clear the cart
                await _cartRepository.ClearCartAsync(cart.CartId);

                _logger.LogInformation("Order created successfully. Order ID: {OrderId}", savedOrder.OrderId);

                // Handle payment method specific logic
                string? paymentUrl = null;
                if (paymentMethod.ToLower() == "vnpay")
                {
                    paymentUrl = GenerateVnPayUrl(savedOrder.OrderId, finalTotalAmount);
                }

                return new CreateOrderResultDto
                {
                    Success = true,
                    Message = "Đặt hàng thành công",
                    OrderId = savedOrder.OrderId,
                    PaymentUrl = paymentUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user ID: {UserId}", userId);
                return new CreateOrderResultDto
                {
                    Success = false,
                    Message = "Lỗi khi tạo đơn hàng"
                };
            }
        }

        private OrderDto MapToOrderDto(Order order)
        {
            // Calculate subtotal from order details
            var subtotal = order.OrderDetails?.Sum(od => (od.Price ?? 0) * (od.Quantity ?? 0)) ?? 0;
            
            // TotalAmount is the final amount including shipping fee
            var totalAmount = order.AmountDue ?? 0;
            
            return new OrderDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                TotalQuantity = order.TotalQuantity,
                AmountDue = order.AmountDue,
                PaymentMethodId = order.PaymentMethodId,
                PaymentMethodName = order.PaymentMethod?.PaymentMethodTittle,
                OrderStatusId = order.OrderStatusId,
                OrderStatusName = order.OrderStatus?.OrderStatusTittle,
                OrderNote = order.OrderNote,
                ShippingAddress = order.ShippingAddress,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderDetails = order.OrderDetails?.Select(od => MapToOrderDetailDto(od)).ToList() ?? new List<OrderDetailDto>(),
                Subtotal = subtotal,
                ShippingFee = 0, // Not displayed in Order History
                TotalAmount = totalAmount,
                TrackingNumber = null // TODO: Add tracking number when available
            };
        }

        private OrderDetailDto MapToOrderDetailDto(OrderDetail orderDetail)
        {
            return new OrderDetailDto
            {
                OrderDetailId = orderDetail.OrderDetailId,
                OrderId = orderDetail.OrderId,
                ProductId = orderDetail.ProductId,
                ProductName = orderDetail.ProductName,
                Quantity = orderDetail.Quantity,
                Price = orderDetail.Price,
                VariantId = orderDetail.VariantId,
                VariantAttributes = orderDetail.VariantAttributes,
                ProductImage = orderDetail.Product?.ProductImages?.FirstOrDefault()?.ImageUrl ?? "",
            };
        }

        private int GetPaymentMethodId(string paymentMethod)
        {
            return paymentMethod.ToLower() switch
            {
                "cod" => 1,      // Cash on Delivery
                "vnpay" => 2,    // VNPay
                "momo" => 3,     // MoMo
                _ => 1           // Default to COD
            };
        }

        private string GenerateVnPayUrl(int orderId, decimal amount)
        {
            // This is a placeholder - implement actual VNPay integration
            // Convert USD to VND for VNPay (1 USD = 24,500 VND)
            var amountInVnd = amount * 24500;
            return $"https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount={amountInVnd * 100}&vnp_Command=pay&vnp_CreateDate={DateTime.Now:yyyyMMddHHmmss}&vnp_CurrCode=VND&vnp_IpAddr=127.0.0.1&vnp_Locale=vn&vnp_OrderInfo=Thanh+toan+don+hang+{orderId}&vnp_OrderType=other&vnp_ReturnUrl=https://localhost:5001/payment/callback&vnp_TmnCode=DEMO&vnp_TxnRef={orderId}&vnp_Version=2.1.0";
        }
    }
} 