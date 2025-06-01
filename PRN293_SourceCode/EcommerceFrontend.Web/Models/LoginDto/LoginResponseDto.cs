namespace EcommerceFrontend.Web.Models.LoginDto
{
    public class LoginResponseDto
    {
        public string message { get; set; }
        public string? token { get; set; }
        public int? userId { get; set; }
        public string? roleName { get; set; }
        public string? userName { get; set; }
    }
}
