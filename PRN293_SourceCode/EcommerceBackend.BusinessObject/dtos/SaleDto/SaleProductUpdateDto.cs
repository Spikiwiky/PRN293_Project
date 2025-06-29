using EcommerceBackend.BusinessObject.dtos.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.dtos.SaleDto
{
    public class SaleProductUpdateDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        public string? ProductName { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public int? ProductCategoryId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Status must be a non-negative number")]
        public int? Status { get; set; }

        public List<string>? ImageUrls { get; set; }

        public List<ProductVariant>? Variants { get; set; }

        [Required(ErrorMessage = "Updated by is required")]
        public string UpdatedBy { get; set; } = string.Empty;

        // Legacy fields for backward compatibility
        public string? Category { get; set; }

        [StringLength(10, ErrorMessage = "Size cannot exceed 10 characters")]
        public string? Size { get; set; }

        [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
        public string? Color { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative number")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative number")]
        public int? StockQuantity { get; set; }

        public bool? IsFeatured { get; set; }
    }
}
