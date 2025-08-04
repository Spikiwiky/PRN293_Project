using System.ComponentModel.DataAnnotations;

namespace EcommerceFrontend.Web.Models.DTOs
{
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        public bool IsDelete { get; set; }
    }
    public class UserCreateDto
    {
        public int RoleId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Phone { get; set; }
        public string UserName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        public bool IsDelete { get; set; }
    }
    public class UserDto
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Role ID là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Role ID phải lớn hơn 0")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [StringLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự")]
        public string Password { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại không vượt quá 15 ký tự")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Tên người dùng là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên người dùng không được vượt quá 50 ký tự")]
        public string UserName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string Address { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime? CreateDate { get; set; }

        [Range(0, 1, ErrorMessage = "Trạng thái phải là 0 (Inactive) hoặc 1 (Active)")]
        public int Status { get; set; }

        public bool IsDelete { get; set; }
    }
}
