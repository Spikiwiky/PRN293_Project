using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EcommerceBackend.BusinessObject.dtos.Shared;

namespace EcommerceBackend.BusinessObject.dtos.ProductDto
{
    public class ProductDetailDto
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Status { get; set; }

        public bool IsDelete { get; set; }

        public List<string> ImageUrls { get; set; } = new();

        public List<ProductVariant> Variants { get; set; } = new();

        // Helper properties to get first variant's data (for backward compatibility)
        public string? Category => Variants.FirstOrDefault()?.Categories;
        public decimal? Price => Variants.FirstOrDefault()?.Price;
        public string? Size => Variants.FirstOrDefault()?.Size;
        public string? Color => Variants.FirstOrDefault()?.Color;
        public string? VariantId => Variants.FirstOrDefault()?.VariantId;
        public int StockQuantity => Variants.FirstOrDefault()?.StockQuantity ?? 0;
        public bool IsFeatured => Variants.FirstOrDefault()?.IsFeatured ?? false;
    }
} 