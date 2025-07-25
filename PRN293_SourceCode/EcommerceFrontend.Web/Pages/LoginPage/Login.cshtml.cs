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
        public IActionResult OnGet(string token, string userName, string roleName, int? userId)
        {
            // Debug logging for Google login
            Console.WriteLine($"Google login - Token: {token?.Substring(0, 20)}..., UserName: {userName}, RoleName: {roleName}, UserId: {userId}");
            
            if (!string.IsNullOrEmpty(token))
            {
                // Set cookies for user authentication
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // Set to true in production with HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(7),
                    Path = "/" // Ensure cookies are available for all paths
                };

                Response.Cookies.Append("Token", token, cookieOptions);

                // For Google login, use the userId from query parameter or set a default
                var googleUserId = userId ?? 1; // Use provided userId or default to 1
                Response.Cookies.Append("UserId", googleUserId.ToString(), cookieOptions);
                Response.Cookies.Append("UserName", userName, cookieOptions);
                Response.Cookies.Append("RoleName", roleName, cookieOptions);

                // Debug logging
                Console.WriteLine($"Google login - Cookies set: Token={token?.Substring(0, 20)}..., UserId={googleUserId}, UserName={userName}, RoleName={roleName}");

                ViewData["Token"] = token;
                ViewData["UserName"] = userName;
                ViewData["RoleName"] = roleName;

                // Redirect to homepage after successful Google login
                return RedirectToPage("/CommonPage/Homepage");
            }

            return Page();
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

                // Debug logging
                Console.WriteLine($"Login response: {System.Text.Json.JsonSerializer.Serialize(response)}");

                if (response == null || response.message != "successful")
                {
                    ErrorMessage = response?.message ?? "Đăng nhập thất bại. Vui lòng thử lại.";
                    return Page();
                }

                // Debug logging
                Console.WriteLine($"Setting cookies - UserId: {response.userId}, Token: {response.token?.Substring(0, 20)}...");

                // Set cookies for user authentication
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // Set to true in production with HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(7),
                    Path = "/" // Ensure cookies are available for all paths
                };

                Response.Cookies.Append("Token", response.token, cookieOptions);
                Response.Cookies.Append("UserId", response.userId.ToString(), cookieOptions);
                Response.Cookies.Append("UserName", response.userName, cookieOptions);
                Response.Cookies.Append("RoleName", response.roleName, cookieOptions);

                // Debug logging
                Console.WriteLine($"Traditional login - Cookies set: Token={response.token?.Substring(0, 20)}..., UserId={response.userId}, UserName={response.userName}, RoleName={response.roleName}");

                // Provide token and info for frontend JavaScript to handle
                ViewData["Token"] = response.token;
                ViewData["RoleName"] = response.roleName;
                ViewData["UserName"] = response.userName;

                // Redirect to homepage after successful login
                return RedirectToPage("/CommonPage/Homepage");
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
