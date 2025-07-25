using System;
using System.Collections.Generic;

namespace EcommerceBackend.DataAccess.Models
{
    public partial class Blog
    {
        public Blog()
        {
            Comments = new HashSet<BlogComment>();
        }

        public int BlogId { get; set; }
        public int? BlogCategoryId { get; set; }
        public string? BlogTittle { get; set; }
        public string? BlogContent { get; set; }
        public string? BlogSummary { get; set; }
        public string? BlogImage { get; set; }
        public int? UserId { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsPublished { get; set; } = false;
        public int ViewCount { get; set; } = 0;
        public string? Tags { get; set; }

        public virtual BlogCategory? BlogCategory { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<BlogComment> Comments { get; set; }
    }
}
