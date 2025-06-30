using System.ComponentModel.DataAnnotations;

namespace EcommerceFrontend.Web.Models.Sale
{
    public class ProductModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Product Category ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product Category ID must be greater than 0.")]
        public int? ProductCategoryId { get; set; }

        [Required(ErrorMessage = "Brand is required.")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Base Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Base Price must be non-negative.")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Available Attributes is required.")]
        public string AvailableAttributes { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Range(0, 1, ErrorMessage = "Status must be 0 or 1.")]
        public int? Status { get; set; }

        public bool IsDelete { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ProductImageModel> ProductImages { get; set; } = new List<ProductImageModel>();
        public List<ProductVariantModel> Variants { get; set; } = new List<ProductVariantModel>();
    }

    public class ProductImageModel
    {
        public string ImageUrl { get; set; }
    }

    public class ProductVariantModel
    {
        public string Attributes { get; set; }
        public string Variants { get; set; }
    }
}
