namespace EcommerceBackend.BusinessObject.dtos.AuthDto
{
    public class TokenDto
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
} 