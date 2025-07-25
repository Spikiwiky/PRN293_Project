using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceBackend.DataAccess.Models
{
    [Table("BlogComment")]
    public partial class BlogComment
    {
        [Key]
        public int CommentId { get; set; }
        public int BlogId { get; set; }
        public string? CommenterName { get; set; }
        public string? CommenterEmail { get; set; }
        public string? CommenterWebsite { get; set; }
        public string? CommentContent { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsApproved { get; set; } = false;

        public virtual Blog? Blog { get; set; }
    }
} 