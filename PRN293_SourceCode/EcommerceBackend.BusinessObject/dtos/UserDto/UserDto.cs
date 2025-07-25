using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.dtos.UserDto
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
    }
    public class EditProfileViewModel
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;

        public string Address { get; set; } = string.Empty;
    }

}
