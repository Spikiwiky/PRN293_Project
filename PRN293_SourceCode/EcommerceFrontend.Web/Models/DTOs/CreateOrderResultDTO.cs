namespace EcommerceFrontend.Web.Models.DTOs
{
    public class CreateOrderResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public string? PaymentUrl { get; set; }
    }
} 