using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.dtos.AuthDto
{
    public class RegisterRequestDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
    }
}
