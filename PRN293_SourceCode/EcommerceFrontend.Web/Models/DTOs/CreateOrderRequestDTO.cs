namespace EcommerceFrontend.Web.Models.DTOs
{
    public class CreateOrderRequestDTO
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public string? OrderNote { get; set; }
        public string? ShippingAddress { get; set; }
        
        // GHN Address Information
        public int? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public int? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public string? WardCode { get; set; }
        public string? WardName { get; set; }
        
        // Order Amount Information
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
    }
} 