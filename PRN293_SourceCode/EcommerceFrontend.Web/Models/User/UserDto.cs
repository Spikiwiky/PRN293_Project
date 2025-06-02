using System.ComponentModel.DataAnnotations;

namespace EcommerceFrontend.Web.Models.User
{
    public class UserDto
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

        // public object Role { get; set; }
        // public List<object> Carts { get; set; }
        // public List<object> Orders { get; set; }
    }
}
