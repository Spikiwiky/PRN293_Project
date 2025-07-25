using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.dtos.SaleDto
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
    public class BlogDetailDto
    {
        public int BlogId { get; set; }
        public string? BlogTittle { get; set; }
        public string? BlogContent { get; set; }
        public string? BlogSummary { get; set; }
        public string? BlogImage { get; set; }
        public string? BlogCategoryTitle { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool IsPublished { get; set; }
        public int ViewCount { get; set; }
        public List<CommentDto>? Comments { get; set; }
    }
    public class CommentDto
    {
        public int CommentId { get; set; }
        public string? CommenterName { get; set; }
        public string? CommentContent { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsApproved { get; set; }
    }

    public class BlogCategoryDto
    {
        public int BlogCategoryId { get; set; }
        public string? BlogCategoryTitle { get; set; }
    }
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class ApproveCommentDto
    {
        public bool IsApproved { get; set; }
    }
    public class BlogCreateUpdateDto
    {
        public int BlogId { get; set; }
        public string BlogTittle { get; set; } = "";
        public string? BlogContent { get; set; }
        public string? BlogSummary { get; set; }
        public string? BlogImage { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool IsPublished { get; set; }
    }
}

