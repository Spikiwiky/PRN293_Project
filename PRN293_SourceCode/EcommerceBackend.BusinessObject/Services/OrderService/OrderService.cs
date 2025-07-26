using EcommerceBackend.BusinessObject.dtos.OrderDto;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return orders.Select(MapToOrderDto).ToList();
        }

        public async Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return orders.Select(MapToOrderDto).ToList();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            return order != null ? MapToOrderDto(order) : null;
        }

        public async Task<List<OrderDetailDto>> GetAllOrderDetailsAsync()
        {
            var orderDetails = await _orderRepository.GetAllOrderDetailsAsync();
            return orderDetails.Select(MapToOrderDetailDto).ToList();
        }

        public async Task<List<OrderDetailDto>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            var orderDetails = await _orderRepository.GetOrderDetailsByOrderIdAsync(orderId);
            return orderDetails.Select(MapToOrderDetailDto).ToList();
        }

        public async Task<CreateOrderResultDto> CreateOrderFromCartAsync(
            int userId, string paymentMethod, string? orderNote, string? shippingAddress,
            int? provinceId = null, string? provinceName = null, int? districtId = null,
            string? districtName = null, string? wardCode = null, string? wardName = null,
            decimal subtotal = 0, decimal shippingFee = 0, decimal totalAmount = 0)
        {
            try
            {
                _logger.LogInformation("Creating order from cart for user ID: {UserId}", userId);

                var cart = await _cartRepository.GetCartByCustomerIdAsync(userId);
                if (cart == null)
                    return new CreateOrderResultDto { Success = false, Message = "Giỏ hàng không tồn tại" };

                var cartDetails = await _orderRepository.GetCartDetailsByCartIdAsync(cart.CartId);
                if (!cartDetails.Any())
                    return new CreateOrderResultDto { Success = false, Message = "Giỏ hàng trống" };

                var calculatedSubtotal = cartDetails.Sum(item => (item.Price ?? 0) * (item.Quantity ?? 0));
                var finalSubtotal = subtotal > 0 ? subtotal : calculatedSubtotal;
                var finalShippingFee = shippingFee;
                var finalTotalAmount = totalAmount > 0 ? totalAmount : (finalSubtotal + finalShippingFee);
                var totalQuantity = cartDetails.Sum(item => item.Quantity ?? 0);

                var completeShippingAddress = shippingAddress;
                if (!string.IsNullOrEmpty(provinceName) && !string.IsNullOrEmpty(districtName) && !string.IsNullOrEmpty(wardName))
                {
                    completeShippingAddress = $"{shippingAddress}, {wardName}, {districtName}, {provinceName}".Trim();
                }

                var order = new Order
                {
                    CustomerId = userId,
                    TotalQuantity = totalQuantity,
                    AmountDue = finalTotalAmount,
                    OrderNote = orderNote,
                    ShippingAddress = completeShippingAddress,
                    PaymentMethodId = GetPaymentMethodId(paymentMethod),
                    OrderStatusId = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var savedOrder = await _orderRepository.CreateOrderAsync(order);
                if (savedOrder == null)
                    return new CreateOrderResultDto { Success = false, Message = "Không thể tạo đơn hàng" };

                var orderDetails = cartDetails.Select(item => new OrderDetail
                {
                    OrderId = savedOrder.OrderId,
                    ProductId = item.ProductId ?? 0,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity ?? 0,
                    Price = item.Price ?? 0,
                    VariantId = item.VariantId?.ToString(),    // Convert to string
                    VariantAttributes = item.VariantAttributes
                }).ToList();

                await _orderRepository.CreateOrderDetailsAsync(orderDetails);
                await _cartRepository.ClearCartAsync(cart.CartId);

                string? paymentUrl = paymentMethod.ToLower() == "vnpay"
                    ? GenerateVnPayUrl(savedOrder.OrderId, finalTotalAmount)
                    : null;

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
                return new CreateOrderResultDto { Success = false, Message = "Lỗi khi tạo đơn hàng" };
            }
        }

        private OrderDto MapToOrderDto(Order order)
        {
            var subtotal = order.OrderDetails?.Sum(od => (od.Price ?? 0) * (od.Quantity ?? 0)) ?? 0;
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
                OrderDetails = order.OrderDetails?.Select(MapToOrderDetailDto).ToList() ?? new List<OrderDetailDto>(),
                Subtotal = subtotal,
                ShippingFee = 0,
                TotalAmount = totalAmount,
                TrackingNumber = null
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
                VariantId = orderDetail.VariantId?.ToString(),   // Ensure always string
                VariantAttributes = orderDetail.VariantAttributes,
                ProductImage = orderDetail.Product?.ProductImages?.FirstOrDefault()?.ImageUrl ?? ""
            };
        }

        private int GetPaymentMethodId(string paymentMethod)
        {
            return paymentMethod.ToLower() switch
            {
                "cod" => 1,
                "vnpay" => 2,
                "momo" => 3,
                _ => 1
            };
        }

        private string GenerateVnPayUrl(int orderId, decimal amount)
        {
            var amountInVnd = amount * 24500;
            return $"https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount={amountInVnd * 100}&vnp_Command=pay&vnp_CreateDate={DateTime.Now:yyyyMMddHHmmss}&vnp_CurrCode=VND&vnp_IpAddr=127.0.0.1&vnp_Locale=vn&vnp_OrderInfo=Thanh+toan+don+hang+{orderId}&vnp_OrderType=other&vnp_ReturnUrl=https://localhost:5001/payment/callback&vnp_TmnCode=DEMO&vnp_TxnRef={orderId}&vnp_Version=2.1.0";
        }
    }
}
