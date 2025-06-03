using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using EcommerceFrontend.Web.Services;
using EcommerceFrontend.Web.Models.RegisterDto;

namespace EcommerceFrontend.Web.Pages.RegisterPage
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientService _httpClientService;

        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email là bắt buộc.")]
            [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
            [StringLength(255, ErrorMessage = "Email must be at most 255 characters long.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
            [DataType(DataType.Password)]
            [StringLength(255, ErrorMessage = "Password must be at most 255 characters long.")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Tên người dùng là bắt buộc.")]
            [StringLength(255, ErrorMessage = "Username must be at most 255 characters long.")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
            [StringLength(20, ErrorMessage = "Phone must be at most 20 characters long.")]
            public string Phone { get; set; }

            public DateTime? DateOfBirth { get; set; }

            [StringLength(500, ErrorMessage = "Address must be at most 500 characters long.")]
            public string? Address { get; set; }
        }

        public RegisterModel(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            Input = new InputModel();
        }

        public void OnGet()
        {
            // Khởi tạo trang
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Tạo dữ liệu gửi đến API
                var registerRequest = new RegisterRequestDto
                {
                    Email = Input.Email?.Trim(),
                    Password = Input.Password?.Trim(),
                    UserName = Input.UserName?.Trim(),
                    Phone = Input.Phone?.Trim(),
                    DateOfBirth = Input.DateOfBirth,
                    Address = Input.Address?.Trim()
                };

                // Gọi API register
                var response = await _httpClientService.PostAsync<RegisterResponseDto>("api/auth/register", registerRequest);

                // Kiểm tra phản hồi từ API
                if (response == null || response.Message != "successful")
                {
                    ErrorMessage = response?.Message ?? "Đăng ký thất bại. Vui lòng thử lại.";
                    return Page();
                }

                // Lưu thông tin vào ViewData để JavaScript sử dụng
                ViewData["Token"] = response.Token;
                ViewData["UserId"] = response.UserId;
                ViewData["RoleName"] = response.RoleName;
                ViewData["UserName"] = response.UserName;

                // Không chuyển hướng ở đây, JavaScript sẽ xử lý
                return Page();
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return Page();
            }
        }
    }
}