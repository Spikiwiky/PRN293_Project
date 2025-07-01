namespace EcommerceFrontend.Web.Models.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? AmountDue { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? OrderStatusId { get; set; }
        public string? OrderNote { get; set; }
    }
} 