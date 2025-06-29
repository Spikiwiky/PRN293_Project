using EcommerceBackend.BusinessObject.dtos.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.dtos.SaleDto
{
    public class SaleProductDto
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product category is required")]
        public int ProductCategoryId { get; set; }

        public string ProductCategoryTitle { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Status must be a non-negative number")]
        public int Status { get; set; }

        public bool IsDelete { get; set; }

        public List<string> ImageUrls { get; set; } = new();

        [Required(ErrorMessage = "At least one variant is required")]
        [MinLength(1, ErrorMessage = "At least one variant is required")]
        public List<ProductVariant> Variants { get; set; } = new();

        // Audit fields
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // Legacy fields for backward compatibility
        public string? Category => Variants.FirstOrDefault()?.Categories;
        public decimal Price => Variants.FirstOrDefault()?.Price ?? 0;
        public string? Size => Variants.FirstOrDefault()?.Size;
        public string? Color => Variants.FirstOrDefault()?.Color;
        public int StockQuantity => Variants.FirstOrDefault()?.StockQuantity ?? 0;
        public bool IsFeatured => Variants.FirstOrDefault()?.IsFeatured ?? false;
    }
}

