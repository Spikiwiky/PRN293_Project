using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceBackend.DataAccess.Models
{
    [Table("reviews")]
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("review_id")]
        public int ReviewId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("content")]
        [StringLength(1000)]
        public string Content { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("is_visible")]
        public bool IsVisible { get; set; } = true;

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
} 