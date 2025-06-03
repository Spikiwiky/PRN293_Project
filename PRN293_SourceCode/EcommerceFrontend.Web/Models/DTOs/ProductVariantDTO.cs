namespace EcommerceFrontend.Web.Models.DTOs
{
    public class ProductVariantDTO
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
} 