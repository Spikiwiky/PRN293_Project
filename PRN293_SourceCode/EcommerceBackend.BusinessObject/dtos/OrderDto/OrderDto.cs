using System;
using System.Collections.Generic;

namespace EcommerceBackend.BusinessObject.dtos.OrderDto
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? AmountDue { get; set; }
        public int? PaymentMethodId { get; set; }
        public string? PaymentMethodName { get; set; }
        public int? OrderStatusId { get; set; }
        public string? OrderStatusName { get; set; }
        public string? OrderNote { get; set; }
        public string? ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
        public decimal? Subtotal { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? TrackingNumber { get; set; }
    }
} 