using System;
using System.Collections.Generic;

namespace EcommerceBackend.DataAccess.Models
{
    public class BlogCategory
    {
        public int BlogCategoryId { get; set; }
        public string BlogCategoryTitle { get; set; }
        public bool IsDelete { get; set; } 

        // Navigation property
        public ICollection<Blog> Blogs { get; set; }
    }
}
