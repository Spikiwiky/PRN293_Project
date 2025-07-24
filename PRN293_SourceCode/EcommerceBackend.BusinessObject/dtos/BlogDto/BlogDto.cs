using EcommerceBackend.BusinessObject.dtos;
using System;
using System.Collections.Generic;

namespace EcommerceBackend.BusinessObject.Dtos
{
    public class BlogDto
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string ContentPreview { get; set; }
        public DateTime CreatedAt { get; set; }
        public BlogCategoryDto Category { get; set; }

        public bool IsDelete { get; set; } 
    }

    public class BlogDetailDto : BlogDto
    {
        public string Content { get; set; }
        public List<CommentDto> Comments { get; set; }
    }

    public class CreateBlogDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int? BlogCategoryId { get; set; }
    }

    public class UpdateBlogDto
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? BlogCategoryId { get; set; }
    }
}