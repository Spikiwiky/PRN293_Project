using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using EcommerceFrontend.Web.Models.User;
using EcommerceFrontend.Web.Services.User;

namespace EcommerceFrontend.Web.Pages.User
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _http;
        private readonly IUserService _userService;

        public EditModel(IHttpClientFactory http, IUserService userService)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [BindProperty]
        public UserDto User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _http.CreateClient("MyAPI");
            var response = await client.GetAsync($"api/Users/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng với ID này.";
                return RedirectToPage("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            User = JsonSerializer.Deserialize<UserDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var success = await _userService.UpdateUserAsync(User);
                if (success)
                {
                    return RedirectToPage("Index");
                }

                ModelState.AddModelError(string.Empty, "Không thể cập nhật người dùng. Vui lòng thử lại.");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
                return Page();
            }
        }
    }
}
