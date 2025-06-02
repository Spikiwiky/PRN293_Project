using System.Text.Json;
using System.Text.Json.Serialization;

namespace EcommerceFrontend.Web.Models.DTOs;

public class IntToBooleanConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32() != 0;
        }
        return reader.GetBoolean();
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value ? 1 : 0);
    }
}

public class ProductVariants
{
    public string Size { get; set; } = string.Empty;
    public string Categories { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class ProductDTO
{
    [JsonPropertyName("productId")]
    public int ProductId { get; set; }

    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("productCategoryId")]
    public int ProductCategoryId { get; set; }

    [JsonPropertyName("productCategoryTitle")]
    public string ProductCategoryTitle { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public string Size { get; set; } = string.Empty;

    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("variant_id")]
    public string VariantId { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonPropertyName("isDelete")]
    public bool? IsDelete { get; set; }

    [JsonPropertyName("imageUrls")]
    public List<string> ImageUrls { get; set; } = new();

    // Helper property to get first image if available
    public string? Image => ImageUrls.FirstOrDefault();

    // Helper property to display category name
    public string? CategoryName => ProductCategoryTitle;

    // Helper property for checking if product is active
    public bool IsActive => Status == 1 && IsDelete != true;

    // Helper property to map with Variants in view
    public ProductVariants Variants => new()
    {
        Size = Size,
        Color = Color,
        Categories = Category
    };
}