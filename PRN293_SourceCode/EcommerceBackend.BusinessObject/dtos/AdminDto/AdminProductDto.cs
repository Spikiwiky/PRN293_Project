using System;
using System.Collections.Generic;

namespace EcommerceBackend.BusinessObject.dtos.AdminDto
{
    public class AdminProductDto
    {
        public int ProductId { get; set; }
        
        public string ProductName { get; set; } = string.Empty;
        
        public int? ProductCategoryId { get; set; }
        
        public string? ProductCategoryTitle { get; set; }
        
        public string? Description { get; set; }
        
        public string? Size { get; set; }
        
        public string? Color { get; set; }
        
        public string? Category { get; set; }
        
        public string? VariantId { get; set; }
        
        public decimal? Price { get; set; }
        
        public int Status { get; set; }
        
        public bool IsDelete { get; set; }
        
        public List<string> ImageUrls { get; set; } = new();

        // Additional admin-specific properties
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int StockQuantity { get; set; }
        public bool IsFeatured { get; set; }
    }
} 