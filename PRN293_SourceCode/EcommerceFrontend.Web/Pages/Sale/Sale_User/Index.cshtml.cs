using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using EcommerceFrontend.Web.Models.DTOs;
namespace EcommerceFrontend.Web.Pages.Sale.Sale_User
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public List<UserResponseDto> Users { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetAsync("api/saleuser");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Users = JsonSerializer.Deserialize<List<UserResponseDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                ErrorMessage = $"Lỗi khi tải danh sách user: {response.ReasonPhrase}";
            }
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/saleuser/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ErrorMessage = $"Lỗi xoá user: {errorContent}";
            await OnGetAsync(); // load lại danh sách
            return Page();
        }

    }
}
