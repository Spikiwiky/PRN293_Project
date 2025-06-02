using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using EcommerceFrontend.Web.Models.User;
using EcommerceFrontend.Web.Services.User;
namespace EcommerceFrontend.Web.Pages.User
{
    public class CreateModel : PageModel
    {
        private readonly IUserService _userService;

        public CreateModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public UserDto User { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var success = await _userService.CreateUserAsync(User);
            if (success)
                return RedirectToPage("Index");

            ModelState.AddModelError(string.Empty, "Tạo user thất bại.");
            return Page();
        }
    }
}