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
    }
} 