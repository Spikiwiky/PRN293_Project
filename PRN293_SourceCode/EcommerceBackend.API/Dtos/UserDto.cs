using EcommerceBackend.DataAccess.Models;

namespace EcommerceBackend.API.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        public bool IsDelete { get; set; }
        public object Role { get; set; }  
        public List<Cart> Carts { get; set; }
        public List<Order> Orders { get; set; }
    }

}
