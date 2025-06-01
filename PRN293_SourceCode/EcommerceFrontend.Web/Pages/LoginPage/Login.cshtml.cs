using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EcommerceFrontend.Web.Services;
using EcommerceFrontend.Web.Models.LoginDto;

namespace EcommerceFrontend.Web.Pages.LoginPage
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientService _httpClientService;

        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email là bắt buộc.")]
            [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public LoginModel(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public void OnGet()
        {
            // Khởi tạo trang (nếu cần)
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
                var loginRequest = new LoginRequestDto
                {
                    Email = Input.Email.Trim(),
                    Password = Input.Password.Trim()
                };

                // Gọi API login
                var response = await _httpClientService.PostAsync<LoginResponseDto>("api/auth/login", loginRequest);

                // Kiểm tra phản hồi từ API
                if (response == null || response.message != "successful")
                {
                    ErrorMessage = response?.message ?? "Đăng nhập thất bại. Vui lòng thử lại.";
                    return Page();
                }

                // Lưu thông tin vào ViewData để JavaScript sử dụng
                ViewData["Token"] = response.token;
                ViewData["RoleName"] = response.roleName;
                ViewData["UserName"] = response.userName;

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