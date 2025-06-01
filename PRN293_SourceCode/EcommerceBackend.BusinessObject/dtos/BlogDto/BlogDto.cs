using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.dtos.BlogDto
{
    public class BlogDto  
    {
        public int BlogId { get; set; }
        public int? BlogCategoryId { get; set; }
        public string? BlogTittle { get; set; }
        public string? BlogContent { get; set; }
    }
}
