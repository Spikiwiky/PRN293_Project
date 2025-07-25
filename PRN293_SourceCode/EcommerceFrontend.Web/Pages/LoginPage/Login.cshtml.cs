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

        // Handle redirect from Google login with query parameters
        public IActionResult OnGet(string token, string userName, string roleName)
        {
            if (!string.IsNullOrEmpty(token))
            {
                // Set cookies for user authentication
                Response.Cookies.Append("Token", token, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // Set to true in production with HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(7)
                });

                Response.Cookies.Append("UserName", userName, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // Set to true in production with HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(7)
                });

                Response.Cookies.Append("RoleName", roleName, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // Set to true in production with HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(7)
                });

                ViewData["Token"] = token;
                ViewData["UserName"] = userName;
                ViewData["RoleName"] = roleName;

                // DO NOT return Redirect here again!
            }

            return Page(); // Important: only return Page, not another redirect
        }

        // Handle traditional login via form submission
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var loginRequest = new LoginRequestDto
                {
                    Email = Input.Email.Trim(),
                    Password = Input.Password.Trim()
                };

                var response = await _httpClientService.PostAsync<LoginResponseDto>("api/auth/login", loginRequest);

                if (response == null || response.message != "successful")
                {
                    ErrorMessage = response?.message ?? "Đăng nhập thất bại. Vui lòng thử lại.";
                    return Page();
                }

                // Set cookies for user authentication
                Response.Cookies.Append("Token", response.token, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // Set to true in production with HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(7)
                });

                Response.Cookies.Append("UserName", response.userName, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // Set to true in production with HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(7)
                });

                Response.Cookies.Append("RoleName", response.roleName, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // Set to true in production with HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(7)
                });

                // Provide token and info for frontend JavaScript to handle
                ViewData["Token"] = response.token;
                ViewData["RoleName"] = response.roleName;
                ViewData["UserName"] = response.userName;

                return Page(); // Let JavaScript handle redirect and storage
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Lỗi khi gọi API: {ex.Message}";
                return Page();
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
                return Page();
            }
        }
    }
}
