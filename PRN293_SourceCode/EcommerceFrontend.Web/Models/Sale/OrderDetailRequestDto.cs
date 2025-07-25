using System.ComponentModel.DataAnnotations;

namespace EcommerceFrontend.Web.Models.DTOs
{
    public class OrderDetailRequestDto
    {
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public string VariantId { get; set; }
    }

    public class OrderDetailResponseDto
    {
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public string VariantId { get; set; }
        public decimal? Price { get; set; }
        public string ProductName { get; set; }
    }

    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? AmountDue { get; set; }
        public int? PaymentMethodId { get; set; }
        public string? OrderNote { get; set; }
        public int? OrderStatusId { get; set; }
        public List<OrderDetailResponseDto> OrderDetails { get; set; }
    }

    public class CreateOrderDto
    {
        public int CustomerId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? OrderStatusId { get; set; }
        public string OrderNote { get; set; }
        public List<OrderDetailRequestDto> OrderDetails { get; set; } = new List<OrderDetailRequestDto>();
    }

    public class UpdateOrderDto
    {
        public int? CustomerId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? OrderStatusId { get; set; }
        public List<OrderDetailRequestDto> OrderDetails { get; set; } = new List<OrderDetailRequestDto>();
    }
    public class OrderDetailDto
    {
        [Required]
        public int? ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int? Quantity { get; set; }

        public string VariantId { get; set; }
    }
}