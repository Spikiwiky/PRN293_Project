using System;

namespace EcommerceBackend.API.Dtos
{
    public class AdminProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public bool IsFeatured { get; set; }
        public int Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDelete { get; set; }
    }
} 