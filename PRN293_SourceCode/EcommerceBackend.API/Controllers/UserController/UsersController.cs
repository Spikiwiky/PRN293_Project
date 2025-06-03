using EcommerceBackend.BusinessObject.dtos.UserDto;
using EcommerceBackend.BusinessObject.Services.UserService;
using EcommerceBackend.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceBackend.API.Controllers.UserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _service.GetById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserDto user)
        {
            _service.Add(user);
            return CreatedAtAction(nameof(Get), new { id = user.UserId }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UserDto userDto)
        {
            if (id != userDto.UserId) return BadRequest();
            _service.Update(userDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }
}
