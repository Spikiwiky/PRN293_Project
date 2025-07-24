using System.Text.Json.Serialization;

namespace EcommerceFrontend.Web.Models.DTOs
{
    public class BlogCategoryDto
    {
        public int BlogCategoryId { get; set; }
        public string BlogCategoryTitle { get; set; }
        public bool IsDelete { get; set; }
    }

    public class CreateBlogCategoryDto
    {
        public string BlogCategoryTitle { get; set; }
    }

    public class UpdateBlogCategoryDto
    {
        public int BlogCategoryId { get; set; }

        public bool IsDelete { get; set; }
        public string BlogCategoryTitle { get; set; }
    }
}