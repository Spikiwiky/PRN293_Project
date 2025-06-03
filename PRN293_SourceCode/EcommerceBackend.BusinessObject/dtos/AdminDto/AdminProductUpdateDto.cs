using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EcommerceBackend.BusinessObject.dtos.Shared;

namespace EcommerceBackend.BusinessObject.dtos.AdminDto
{
    public class AdminProductUpdateDto
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
    }
} 