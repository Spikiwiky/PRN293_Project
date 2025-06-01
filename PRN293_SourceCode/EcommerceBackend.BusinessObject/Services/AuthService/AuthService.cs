using EcommerceBackend.BusinessObject.Abstract.AuthAbstract;
using EcommerceBackend.BusinessObject.dtos.AuthDto;
using EcommerceBackend.DataAccess.Abstract.AuthAbstract;
using EcommerceBackend.DataAccess.Repository.AuthRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public string GenerateJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleName)

                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Hàm kiểm tra định dạng email hợp lệ
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public UserDto? ValidateUser(LoginRequestDto loginRequest)
        {

            var user = _authRepository.GetUserByEmail(loginRequest.Email);
            if (user == null || user.Password != loginRequest.Password || user.Status != 1 || user.IsDelete == true)
            {
                return null;
            }
            if (user.Status != 1 || user.IsDelete == true)
            {
                throw new UnauthorizedAccessException();

            }

            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                RoleName = user.Role.RoleName ?? "",
                UserName = user.UserName ?? "",
            };
        }
    }
}
