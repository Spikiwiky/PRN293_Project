using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using EcommerceFrontend.Web.Models.DTOs;
namespace EcommerceFrontend.Web.Pages.Sale.Sale_User
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        [BindProperty]
        public UserCreateDto UserDto { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Dữ liệu không hợp lệ.";
                return Page();
            }

            var json = JsonSerializer.Serialize(UserDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/saleuser", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Sale/Sale_User/Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ErrorMessage = $"Lỗi tạo user: {errorContent}";
            return Page();
        }
    }
}
