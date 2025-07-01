namespace EcommerceBackend.BusinessObject.dtos
{
    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public string? VariantId { get; set; }
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
} 