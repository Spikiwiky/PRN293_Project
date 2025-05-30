using System;
using System.Collections.Generic;

namespace EcommerceBackend.DataAccess.Models
{
    public partial class BlogCategory
    {
        public BlogCategory()
        {
            Blogs = new HashSet<Blog>();
        }

        public int BlogCategoryId { get; set; }
        public string? BlogCategoryTitle { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
    }
}
