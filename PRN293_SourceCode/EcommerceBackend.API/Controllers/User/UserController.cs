using EcommerceBackend.BusinessObject.Services.User;
using EcommerceBackend.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBackend.API.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly EcommerceDBContext _context;

        public UsersController(EcommerceDBContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EcommerceBackend.DataAccess.Models.User>>> GetUsers()
        {
            // Nếu bạn không muốn trả về Role, bỏ Include()
            return await _context.Users
                .Include(u => u.Role) // nếu muốn lấy luôn Role
                .Where(u => u.IsDelete != true)
                .ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EcommerceBackend.DataAccess.Models.User>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsDelete != true);

            if (user == null) return NotFound();
            return user;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<EcommerceBackend.DataAccess.Models.User>> PostUser(EcommerceBackend.DataAccess.Models.User user)
        {
            user.CreateDate = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, EcommerceBackend.DataAccess.Models.User user)
        {
            if (id != user.UserId) return BadRequest();

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return NotFound();

            // Cập nhật thủ công để tránh ghi đè navigation
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;
            existingUser.Phone = user.Phone;
            existingUser.UserName = user.UserName;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.Address = user.Address;
            existingUser.RoleId = user.RoleId;
            existingUser.Status = user.Status;

            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Users/5 (soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Soft delete
            user.IsDelete = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
