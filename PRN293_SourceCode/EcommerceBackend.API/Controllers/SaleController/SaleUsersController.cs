using EcommerceBackend.BusinessObject.Services.SaleService.UserService;
using Microsoft.AspNetCore.Mvc;
using EcommerceBackend.BusinessObject.dtos.UserDto;
using EcommerceBackend.API.Dtos.Sale;

namespace EcommerceBackend.API.Controllers.SaleController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleUserController : ControllerBase
    {
        private readonly ISaleUserService _userService;

        public SaleUserController(ISaleUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAll().Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                // If u.RoleId is null, it will default to 0. Adjust default as needed.
                RoleId = u.RoleId ?? 0,
                Email = u.Email,
                Password = u.Password,
                Phone = u.Phone,
                UserName = u.UserName,
                // If u.DateOfBirth is null, it will default to DateTime.MinValue. Adjust default as needed.
                DateOfBirth = u.DateOfBirth ?? DateTime.MinValue,
                Address = u.Address,
                // If u.CreateDate is null, it will default to DateTime.MinValue. Adjust default as needed.
                CreateDate = u.CreateDate ?? DateTime.MinValue,
                // If u.Status is null, it will default to 0. Adjust default as needed.
                Status = u.Status ?? 0,
                // If u.IsDelete is null, it will default to false. Adjust default as needed.
                IsDelete = u.IsDelete ?? false
            });
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }

            var userDto = new UserResponseDto
            {
                UserId = user.UserId,
                RoleId = user.RoleId ?? 0,
                Email = user.Email,
                Password = user.Password,
                Phone = user.Phone,
                UserName = user.UserName,
                DateOfBirth = (DateTime)user.DateOfBirth,
                Address = user.Address,
                CreateDate = (DateTime)user.CreateDate,
                Status = (int)user.Status,
                IsDelete = (bool)user.IsDelete
            };
            return Ok(userDto);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] EcommerceBackend.BusinessObject.dtos.SaleDto.UserCreateDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User data is invalid");
            }

            try
            {
                _userService.Add(userDto);

                // Lấy user vừa tạo để trả về UserId
                var createdUser = _userService.GetAll().OrderByDescending(u => u.UserId).FirstOrDefault(u => u.Email == userDto.Email);
                if (createdUser == null)
                {
                    return StatusCode(500, new { Message = "Failed to retrieve created user" });
                }

                var responseDto = new UserResponseDto
                {
                    UserId = createdUser.UserId,
                    RoleId = (int)createdUser.RoleId,
                    Email = createdUser.Email,
                    Password = createdUser.Password,
                    Phone = createdUser.Phone,
                    UserName = createdUser.UserName,
                    DateOfBirth = (DateTime)createdUser.DateOfBirth,
                    Address = createdUser.Address,
                    CreateDate = (DateTime)createdUser.CreateDate,
                    Status = (int)createdUser.Status,
                    IsDelete = (bool)createdUser.IsDelete
                };

                return CreatedAtAction(nameof(GetUserById), new { id = responseDto.UserId }, responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error creating user", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserDto userDto)
        {
            if (userDto == null || userDto.UserId != id)
            {
                return BadRequest("User data is invalid or ID mismatch");
            }

            try
            {
                _userService.Update(userDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                _userService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound($"User with ID {id} not found or error occurred: {ex.Message}");
            }
        }
    }
}
