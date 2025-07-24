using System.ComponentModel.DataAnnotations;

namespace EcommerceBackend.BusinessObject.dtos
{
    public class BlogCategoryDto
    {
        public int BlogCategoryId { get; set; }
        public string BlogCategoryTitle { get; set; }

        // Int representation to match DB field (bit) but used as bool here
        public bool IsDelete { get; set; }
    }

    public class CreateBlogCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string BlogCategoryTitle { get; set; }

        // Optional, defaults to false (IsDelete = 0)
        public bool IsDelete { get; set; } = false;
    }

    public class UpdateBlogCategoryDto
    {
        [Required]
        public int BlogCategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string BlogCategoryTitle { get; set; }

        public bool IsDelete { get; set; }
    }
}
