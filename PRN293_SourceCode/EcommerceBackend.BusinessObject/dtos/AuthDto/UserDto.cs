using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.dtos.AuthDto
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } = null!;
        public string RoleName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
