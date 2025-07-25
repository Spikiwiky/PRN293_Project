using System.ComponentModel.DataAnnotations;

namespace EcommerceBackend.API.Dtos.Sale
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
}
