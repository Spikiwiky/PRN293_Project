using System.ComponentModel.DataAnnotations;

namespace EcommerceFrontend.Web.Models.DTOs
{
    public class AddVariantDTO
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Size is required")]
        [StringLength(50, ErrorMessage = "Size cannot be longer than 50 characters")]
        public string Size { get; set; } = string.Empty;

        [Required(ErrorMessage = "Color is required")]
        [StringLength(50, ErrorMessage = "Color cannot be longer than 50 characters")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0")]
        public int Stock { get; set; }
    }
} 