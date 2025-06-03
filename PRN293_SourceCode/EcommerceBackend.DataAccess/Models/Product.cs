using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceBackend.DataAccess.Models
{
    public partial class Product
    {
        public Product()
        {
            CartDetails = new HashSet<CartDetail>();
            OrderDetails = new HashSet<OrderDetail>();
            ProductImages = new HashSet<ProductImage>();
        }

        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(200)]
        public string? ProductName { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public int? ProductCategoryId { get; set; }

        [ForeignKey("ProductCategoryId")]
        public virtual ProductCategory? ProductCategory { get; set; }

        public int? Status { get; set; }

        public bool? IsDelete { get; set; }

        public string? Variants { get; set; }

   
        public virtual ICollection<ProductImage>? ProductImages { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
