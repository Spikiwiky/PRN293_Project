namespace EcommerceBackend.BusinessObject.dtos.ProductDto
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? ProductCategoryId { get; set; }
        public string? ProductCategoryName { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public bool? IsDelete { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
} 