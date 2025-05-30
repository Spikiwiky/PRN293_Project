using System;
using System.Collections.Generic;

namespace EcommerceBackend.DataAccess.Models
{
    public partial class Blog
    {
        public int BlogId { get; set; }
        public int? BlogCategoryId { get; set; }
        public string? BlogTittle { get; set; }
        public string? BlogContent { get; set; }

        public virtual BlogCategory? BlogCategory { get; set; }
    }
}
