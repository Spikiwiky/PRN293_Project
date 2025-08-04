using System.ComponentModel.DataAnnotations;

namespace EcommerceFrontend.Web.Models.User
{
    public class UserDto
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Role ID là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Role ID phải lớn hơn 0")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được quá 15 ký tự")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Tên người dùng là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên người dùng không được quá 50 ký tự")]
        public string UserName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được quá 200 ký tự")]
        public string Address { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime? CreateDate { get; set; }

        [Range(0, 1, ErrorMessage = "Trạng thái phải là 0 hoặc 1")]
        public int Status { get; set; }

        public bool IsDelete { get; set; }
    }
}
