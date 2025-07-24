using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceBackend.DataAccess.Models
{
    public class Blog
    {
        [Key]
        [Column("Blog_id")]
        public int BlogId { get; set; }

        [Column("Blog_category_id")]
        public int? BlogCategoryId { get; set; }

        [Column("Blog_tittle")]
        [StringLength(255)]
        public string BlogTittle { get; set; }

        [Column("Blog_content")]
        public string BlogContent { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("IsDelete")]
        public bool IsDelete { get; set; } = false;

        // Navigation properties
        [ForeignKey("BlogCategoryId")]
        public virtual BlogCategory BlogCategory { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}