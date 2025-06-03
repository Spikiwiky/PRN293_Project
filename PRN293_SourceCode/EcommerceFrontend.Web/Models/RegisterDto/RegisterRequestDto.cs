using System.Text.Json.Serialization;

namespace EcommerceFrontend.Web.Models.RegisterDto
{
    public class RegisterRequestDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        [JsonPropertyName("userName")]
        public string UserName { get; set; } = null!;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = null!; // Đã bắt buộc, không cần ?

        [JsonPropertyName("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }
    }
}
