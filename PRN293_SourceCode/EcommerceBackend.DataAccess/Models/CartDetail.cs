using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceBackend.DataAccess.Models
{
    [Table("Cart_detail")]
    public class CartDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Cart_detail_id")]
        public int CartDetailId { get; set; }

        [Column("Cart_id")]
        public int? CartId { get; set; }

        [Column("Product_id")]
        public int? ProductId { get; set; }

        [Column("Variant_id")]
        [StringLength(50)]
        public string VariantId { get; set; }

        [Column("Product_name")]
        [StringLength(255)]
        public string ProductName { get; set; }

        [Column("Quantity")]
        public int? Quantity { get; set; }

        [Column("Price")]
        [Precision(10, 2)]
        public decimal? Price { get; set; }

        [Column("Variant_attributes")]
        public string VariantAttributes { get; set; }

        // Navigation properties
        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
} 