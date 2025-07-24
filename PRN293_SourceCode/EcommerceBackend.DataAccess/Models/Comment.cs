using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Models
{
    [Table("Comment")]
    public partial class Comment
    {
        [Key]
        public int CommentId { get; set; }

        public int BlogId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string Content { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("BlogId")]
        [InverseProperty("Comments")]
        public virtual Blog Blog { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Comments")]
        public virtual User User { get; set; }
    }
}
