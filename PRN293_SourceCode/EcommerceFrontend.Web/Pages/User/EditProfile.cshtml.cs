using EcommerceBackend.BusinessObject.dtos.UserDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

public class EditProfileModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public EditProfileModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [BindProperty]
    public EditProfileViewModel Profile { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        int userId = 5; // Lấy từ Claims hoặc session
        var client = _clientFactory.CreateClient("MyAPI");
        var dto = await client.GetFromJsonAsync<UserDto>($"api/users/{userId}");
        if (dto == null) return NotFound();

        Profile = new EditProfileViewModel
        {
            UserId = dto.UserId,
            Email = dto.Email,
            UserName = dto.UserName,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            Address = dto.Address
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var client = _clientFactory.CreateClient("MyAPI");
        var updateDto = new UserDto
        {
            UserId = Profile.UserId,
            Email = Profile.Email,
            Password = string.IsNullOrWhiteSpace(Profile.Password) ? "******" : Profile.Password,
            Phone = Profile.Phone,
            UserName = Profile.UserName,
            DateOfBirth = Profile.DateOfBirth,
            Address = Profile.Address,
            CreateDate = DateTime.Now,
            RoleId = 2,       // hoặc lấy từ user hiện tại
            Status = 1,
            IsDelete = false
        };

        var response = await client.PutAsJsonAsync($"api/users/{Profile.UserId}", updateDto);
        if (response.IsSuccessStatusCode)
            return RedirectToPage("/Index");

        ModelState.AddModelError("", "Không thể cập nhật hồ sơ.");
        return Page();
    }
}
