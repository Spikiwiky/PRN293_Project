namespace EcommerceFrontend.Web.Models.DTOs
{
    public class BlogDto
    {
        public int BlogId { get; set; }
        public int BlogCategoryId { get; set; }
        public string BlogTittle { get; set; }
        public string BlogContent { get; set; }
        public string? BlogSummary { get; set; }
        public string? BlogImage { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsPublished { get; set; }
        public int ViewCount { get; set; }
        public string? Tags { get; set; }
        public string? BlogCategoryTitle { get; set; }
    }
}
