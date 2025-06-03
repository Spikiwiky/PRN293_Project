using System.ComponentModel.DataAnnotations;

namespace EcommerceBackend.BusinessObject.dtos.ProductDto
{
    public class AddVariantDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(10)]
        public string Size { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public bool IsFeatured { get; set; }

        public string? Categories { get; set; }
    }
} 