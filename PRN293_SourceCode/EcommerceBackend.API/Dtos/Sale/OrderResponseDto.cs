namespace EcommerceBackend.API.Dtos.Sale
{
    public class OrderDetailRequestDto
    {
        public int? ProductId { get; set; }
        public string? VariantId { get; set; }
        public int Quantity { get; set; }
        public Dictionary<string, string>? VariantAttributes { get; set; } 
    }


    public class OrderDto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int TotalQuantity { get; set; } 
        public decimal AmountDue { get; set; }
        public int PaymentMethodId { get; set; } 
        public string OrderNote { get; set; }
        public int OrderStatusId { get; set; }
        public List<OrderDetailResponseDto> OrderDetails { get; set; }
        public string ShippingAddress { get; set; } 
    }
    public class OrderDetailResponseDto
    {
        public int? ProductId { get; set; }
        public string? VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }
        public string? VariantAttributes { get; set; }
        public int? OrderStatusId { get; set; }  
    }

    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? AmountDue { get; set; }
        public int? PaymentMethodId { get; set; }
        public string OrderNote { get; set; }
        public int? OrderStatusId { get; set; }  
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderDetailResponseDto> OrderDetails { get; set; }
    }


    public class CreateOrderDto
    {
        public int CustomerId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? OrderStatusId { get; set; }
        public string OrderNote { get; set; }
        public string ShippingAddress { get; set; } 
        public decimal ShippingFee { get; set; } = 0;
        public List<OrderDetailRequestDto> OrderDetails { get; set; } = new List<OrderDetailRequestDto>();
    }

    public class UpdateOrderDto
    {
        public int? CustomerId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? OrderStatusId { get; set; }
        public string? OrderNote { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderDetailRequestDto> OrderDetails { get; set; } = new List<OrderDetailRequestDto>();
    }
}