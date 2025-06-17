using System.Text.Json.Serialization;

namespace EcommerceBackend.BusinessObject.dtos.Shared
{
    public class AvailableAttributesConfig
    {
        [JsonPropertyName("fields")]
        public int Fields { get; set; }

        [JsonPropertyName("defaults")]
        public Dictionary<string, string> Defaults { get; set; } = new();

        [JsonPropertyName("selectable")]
        public Dictionary<string, bool> Selectable { get; set; } = new();
    }
}