using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceBackend.DataAccess.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("Product_category_id")]
        public int? ProductCategoryId { get; set; }

        [Column("brand")]
        [StringLength(100)]
        public string Brand { get; set; }

        [Column("base_price")]
        [Precision(10, 2)]
        public decimal BasePrice { get; set; }

        [Column("available_attributes")]
        public string AvailableAttributes { get; set; }

        [Column("status")]
        public int? Status { get; set; }

        [Column("is_delete")]
        public bool IsDelete { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ProductCategoryId")]
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<ProductVariant> Variants { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
