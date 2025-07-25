using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceBackend.DataAccess.Models
{
    [Table("ratings")]
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("rating_id")]
        public int RatingId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("score")]
        [Range(1, 5)]
        public int Score { get; set; }

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