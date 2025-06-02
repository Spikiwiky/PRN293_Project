using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcommerceBackend.BusinessObject.dtos.AdminDto
{
    public class AdminProductUpdateDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [StringLength(200, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 200 characters")]
        public string? ProductName { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public int? ProductCategoryId { get; set; }

        [StringLength(50, ErrorMessage = "Size cannot exceed 50 characters")]
        public string? Size { get; set; }

        [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
        public string? Color { get; set; }

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string? Category { get; set; }

        public string? VariantId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to 0")]
        public int? StockQuantity { get; set; }

        public bool? IsFeatured { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Status must be a non-negative number")]
        public int? Status { get; set; }

        public List<string>? ImageUrls { get; set; }

        public string? UpdatedBy { get; set; }
    }
} 