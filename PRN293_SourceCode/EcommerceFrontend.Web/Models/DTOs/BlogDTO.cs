namespace EcommerceFrontend.Web.Models.DTOs
{
    public class BlogDto
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string? ImageUrl { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
