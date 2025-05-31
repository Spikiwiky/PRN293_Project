//using EcommerceBackend.BusinessObject.Abstract;
//using EcommerceBackend.BusinessObject.dtos.AuthDto;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace EcommerceBackend.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly IAuthService _authService;

//        public AuthController(IAuthService authService)
//        {
//            _authService = authService;
//        }

//        [HttpPost("login")]
//        [AllowAnonymous]
//        public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto loginDto)
//        {
//            try
//            {
//                var result = await _authService.LoginAsync(loginDto);
//                return Ok(result);
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                return Unauthorized(new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//        }

//        [HttpPost("register")]
//        [AllowAnonymous]
//        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
//        {
//            try
//            {
//                var result = await _authService.RegisterAsync(registerDto);
//                return Ok(new { message = "Registration successful" });
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//        }

//        [HttpGet("validate")]
//        [Authorize]
//        public ActionResult ValidateToken()
//        {
//            return Ok(new { message = "Token is valid" });
//        }
//    }
//} 