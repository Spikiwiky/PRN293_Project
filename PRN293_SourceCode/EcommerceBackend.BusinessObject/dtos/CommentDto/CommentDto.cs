using System;

namespace EcommerceBackend.BusinessObject.Dtos
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int BlogId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Author { get; set; }
    }

    public class CreateCommentDto
    {
        public int BlogId { get; set; }
        public int? UserId { get; set; }
        public string Content { get; set; }
    }

    public class UpdateCommentDto
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
    }
}