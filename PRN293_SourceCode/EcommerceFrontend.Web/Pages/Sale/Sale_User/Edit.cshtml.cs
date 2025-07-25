using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using EcommerceFrontend.Web.Models.DTOs;
namespace EcommerceFrontend.Web.Pages.Sale.Sale_User
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        [BindProperty]
        public UserDto UserDto { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/saleuser/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (user == null)
                {
                    ErrorMessage = "Không tìm thấy user.";
                    return Page();
                }

                UserDto = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Password = user.Password,
                    UserName = user.UserName,
                    Phone = user.Phone,
                    Address = user.Address,
                    DateOfBirth = user.DateOfBirth,
                    RoleId = user.RoleId,
                    Status = user.Status,
                    IsDelete = user.IsDelete
                };
                return Page();
            }

            ErrorMessage = $"Không thể lấy dữ liệu user. Lỗi: {response.ReasonPhrase}";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Dữ liệu không hợp lệ.";
                return Page();
            }

            UserDto.UserId = id;

            var json = JsonSerializer.Serialize(UserDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/saleuser/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Sale/Sale_User/Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ErrorMessage = $"Lỗi cập nhật user: {errorContent}";
            return Page();
        }
    }
}
