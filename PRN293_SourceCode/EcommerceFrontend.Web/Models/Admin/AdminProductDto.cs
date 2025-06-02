namespace EcommerceFrontend.Web.Models.Admin
{
    public class AdminProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProductCategoryId { get; set; }
        public string? ProductCategoryTitle { get; set; }
        public string? Category { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public bool IsFeatured { get; set; }
        public int Status { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public int StockQuantity { get; set; }
        public string? VariantId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class AdminProductCreateDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProductCategoryId { get; set; }
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public bool IsFeatured { get; set; }
        public int Status { get; set; } = 1;
        public int StockQuantity { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public string? VariantId { get; set; }
    }

    public class AdminProductUpdateDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ProductCategoryId { get; set; }
        public string? Category { get; set; }
        public decimal? Price { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public bool? IsFeatured { get; set; }
        public int? Status { get; set; }
        public int? StockQuantity { get; set; }
        public List<string>? ImageUrls { get; set; }
        public string? VariantId { get; set; }
    }
} 