using EcommerceBackend.BusinessObject.Abstract.AuthAbstract;
using EcommerceBackend.BusinessObject.dtos.AuthDto;
using EcommerceBackend.DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceBackend.API.Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IAuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (loginRequest == null)
                {
                    return BadRequest(new { message = "Invalid request data." });
                }

                if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
                {
                    return BadRequest(new { message = "Email and Password are required." });
                }

                if (loginRequest.Email.Length > 255)
                {
                    return BadRequest(new { message = "Email must be at most 255 characters long." });
                }
                // Kiểm tra email có đúng định dạng không
                if (!_authService.IsValidEmail(loginRequest.Email))
                {
                    return BadRequest(new { message = "Invalid email format." });
                }
                if (loginRequest.Password.Length > 255)
                {
                    return BadRequest(new { message = "Password must be at most 255 characters long." });
                }

                var user = _authService.ValidateUser(loginRequest);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email, password. " });
                }

                var token = _authService.GenerateJwtToken(user);
                // Lưu thông tin vào Session
                var session = _httpContextAccessor.HttpContext.Session;
                session.SetString("Token", token);
                session.SetString("UserId", user.UserId.ToString());
                session.SetString("Email", user.Email);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                return Ok(new { message = "successful", token, userId = user.UserId, RoleName = user.RoleName, UserName = user.UserName });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "deactived account."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while processing your request.",
                    error = ex.Message
                });
            }
        }


        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDto registerRequest)
        {
            if (registerRequest == null)
            {
                return BadRequest(new { message = "Invalid request data." });
            }

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(registerRequest.Email) || string.IsNullOrWhiteSpace(registerRequest.Password) || string.IsNullOrWhiteSpace(registerRequest.UserName))
            {
                return BadRequest(new { message = "Email, Password, and UserName are required." });
            }

            if (registerRequest.Email.Length > 255)
            {
                return BadRequest(new { message = "Email must be at most 255 characters long." });
            }

            if (registerRequest.Password.Length > 255)
            {
                return BadRequest(new { message = "Password must be at most 255 characters long." });
            }

            if (registerRequest.UserName.Length > 255)
            {
                return BadRequest(new { message = "UserName must be at most 255 characters long." });
            }

            if (registerRequest.Phone != null && registerRequest.Phone.Length > 20)
            {
                return BadRequest(new { message = "Phone must be at most 20 characters long." });
            }

            if (registerRequest.Address != null && registerRequest.Address.Length > 500)
            {
                return BadRequest(new { message = "Address must be at most 500 characters long." });
            }
            if (!_authService.IsValidEmail(registerRequest.Email))
            {
                return BadRequest(new { message = "Email address does not match format." });
            }
            try
            {
                var user = _authService.RegisterUser(
                    registerRequest.Email,
                    registerRequest.Password,
                    registerRequest.UserName,
                    registerRequest.Phone,
                    registerRequest.DateOfBirth,
                    registerRequest.Address
                );

                var token = _authService.GenerateJwtToken(user);
                // Lưu thông tin vào Session
                var session = _httpContextAccessor.HttpContext.Session;
                session.SetString("Token", token);
                session.SetString("UserId", user.UserId.ToString());
                session.SetString("Email", user.Email);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                return Ok(new { message = "successful", token, userId = user.UserId, RoleName = user.RoleName, UserName = user.UserName });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi không xác định.", error = ex.Message });
            }
        }






    }
}
