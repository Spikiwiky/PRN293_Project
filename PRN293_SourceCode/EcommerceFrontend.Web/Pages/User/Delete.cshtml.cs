using EcommerceFrontend.Web.Models.User;
using EcommerceFrontend.Web.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace EcommerceFrontend.Web.Pages.User
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _http;
        private readonly IUserService _userService;

        public DeleteModel(IHttpClientFactory http, IUserService userService)
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
                TempData["ErrorMessage"] = "Không tìm thấy người dùng để xóa.";
                return RedirectToPage("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            User = JsonSerializer.Deserialize<UserDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var success = await _userService.DeleteUserAsync(User.UserId);
                if (success)
                {
                    TempData["SuccessMessage"] = "Người dùng đã được xóa thành công.";
                    return RedirectToPage("Index");
                }

                TempData["ErrorMessage"] = "Không thể xóa người dùng. Vui lòng thử lại.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToPage("Index");
            }
        }
    }
}
