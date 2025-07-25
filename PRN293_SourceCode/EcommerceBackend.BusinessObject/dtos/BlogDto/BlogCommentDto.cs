using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.dtos.BlogDto
{
    public class BlogCommentDto
    {
        public int CommentId { get; set; }
        public int BlogId { get; set; }
        public string? CommenterName { get; set; }
        public string? CommenterEmail { get; set; }
        public string? CommenterWebsite { get; set; }
        public string? CommentContent { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsApproved { get; set; }
    }
} 