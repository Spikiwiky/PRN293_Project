using EcommerceBackend.BusinessObject.Abstract.AuthAbstract;
using EcommerceBackend.BusinessObject.dtos.AuthDto;
using EcommerceBackend.DataAccess.Abstract.AuthAbstract;
using EcommerceBackend.DataAccess.Models;
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

        public UserDto RegisterUser(string email, string password, string userName, string? phone, DateTime? dateOfBirth, string? address)
        {
            // Kiểm tra email đã tồn tại chưa
            var existingUser = _authRepository.GetUserByEmail(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            // Tạo mới người dùng
            var user = new User
            {
                Email = email,
                Password = password,
                UserName = userName,
                Phone = phone,
                DateOfBirth = dateOfBirth,
                Address = address,
                RoleId = 3,
                CreateDate = DateTime.Now,
                Status = 1,
                IsDelete = false
            };
            var createdUser = _authRepository.CreateUser(user);
            if (createdUser == null)
            {
                throw new Exception();
            }
            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                RoleName = user.Role.RoleName ?? "",
                UserName = user.UserName ?? "",
            };
        }
        public UserDto RegisterUserByGoogle(string email, string password, string userName, string? phone, DateTime? dateOfBirth, string? address)
        {
            var existingUser = _authRepository.GetUserByEmail(email);
            if (existingUser != null)
            {
                // Just return existing user as UserDto
                return new UserDto
                {
                    UserId = existingUser.UserId,
                    Email = existingUser.Email,
                    RoleName = existingUser.Role?.RoleName ?? "",
                    UserName = existingUser.UserName ?? "",

                };
            }

            // Create new user
            var user = new User
            {
                Email = email,
                Password = password,
                UserName = userName,
                Phone = phone,
                DateOfBirth = dateOfBirth,
                Address = address,
                RoleId = 3,
                CreateDate = DateTime.Now,
                Status = 1,
                IsDelete = false
            };

            var createdUser = _authRepository.CreateUser(user);
            if (createdUser == null)
            {
                throw new Exception("Failed to create user.");
            }

            return new UserDto
            {
                UserId = createdUser.UserId,
                Email = createdUser.Email,
                RoleName = createdUser.Role?.RoleName ?? "",
                UserName = createdUser.UserName ?? "",

            };
        }





    }
}
