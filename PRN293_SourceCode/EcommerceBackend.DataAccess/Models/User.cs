using System;
using System.Collections.Generic;

namespace EcommerceBackend.DataAccess.Models
{
    public partial class User
    {
        public User()
        {
        }

        public int UserId { get; set; }
        public int? RoleId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Phone { get; set; }
        public string? UserName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? Status { get; set; }
        public bool? IsDelete { get; set; }
    }
}
