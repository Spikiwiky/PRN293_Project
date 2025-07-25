using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceBackend.DataAccess.Models
{
    [Table("Cart")]
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Cart_id")]
        public int CartId { get; set; }

        [Column("Customer_id")]
        public int? CustomerId { get; set; }

        [Column("Total_quantity")]
        public int? TotalQuantity { get; set; }

        [Column("Amount_due")]
        [Precision(10, 2)]
        public decimal? AmountDue { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual User Customer { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
} 