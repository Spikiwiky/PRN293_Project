using EcommerceFrontend.Web.Models.DTOs;

namespace EcommerceFrontend.Web.Models.Admin
{
    public class AdminBlogDto
    {
        public int BlogId { get; set; }
        public int BlogCategoryId { get; set; }
        public string BlogTittle { get; set; }
        public string BlogContent { get; set; }
    }
}
