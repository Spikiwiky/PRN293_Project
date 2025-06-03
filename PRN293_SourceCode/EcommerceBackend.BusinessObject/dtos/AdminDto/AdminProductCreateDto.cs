using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EcommerceBackend.BusinessObject.dtos.Shared;

namespace EcommerceBackend.BusinessObject.dtos.AdminDto
{
    public class AdminProductCreateDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product category is required")]
        public int ProductCategoryId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Status must be a non-negative number")]
        public int Status { get; set; }

        public List<string> ImageUrls { get; set; } = new();

        [Required(ErrorMessage = "At least one variant is required")]
        [MinLength(1, ErrorMessage = "At least one variant is required")]
        public List<ProductVariant> Variants { get; set; } = new();

        [Required(ErrorMessage = "Created by is required")]
        public string CreatedBy { get; set; } = string.Empty;

        // Legacy fields for backward compatibility
        public string? Category { get; set; }
        
        [StringLength(10, ErrorMessage = "Size cannot exceed 10 characters")]
        public string? Size { get; set; }
        
        [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
        public string? Color { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative number")]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative number")]
        public int StockQuantity { get; set; }
        
        public bool IsFeatured { get; set; }
    }
} 