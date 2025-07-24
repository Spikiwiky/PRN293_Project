using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace EcommerceFrontend.Web.Models.Admin
{
    public class AdminBlogDto
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ContentPreview { get; set; }
        public DateTime CreatedAt { get; set; }
        public BlogCategoryDto Category { get; set; }
        public bool IsDelete { get; set; }
    }

    public class BlogCategoryDto
    {
        public int BlogCategoryId { get; set; }
        public string BlogCategoryTitle { get; set; }
        public bool IsDelete { get; set; }
    }

    public class AdminUpdateBlogDto
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? BlogCategoryId { get; set; }
    }

 

    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class AdminBlogDetailDto : AdminBlogDto
    {
        [JsonPropertyName("comments")]
        public List<AdminCommentDto> Comments { get; set; } = new List<AdminCommentDto>();
    }

    public class AdminCreateBlogDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int? BlogCategoryId { get; set; }
    }



    public class AdminCommentDto
    {
        [JsonPropertyName("commentId")]
        public int CommentId { get; set; }

        [JsonPropertyName("blogId")]
        public int BlogId { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("isDelete")]
        public bool IsDeleted { get; set; }
    }

    public class AdminCreateCommentDto
    {
        public int BlogId { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
    }

    public class AdminUpdateCommentDto
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
    }
}