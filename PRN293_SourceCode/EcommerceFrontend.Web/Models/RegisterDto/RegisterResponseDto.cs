using System.Text.Json.Serialization;

namespace EcommerceFrontend.Web.Models.RegisterDto
{
    public class RegisterResponseDto
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("roleName")]
        public string RoleName { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }
    }
}
