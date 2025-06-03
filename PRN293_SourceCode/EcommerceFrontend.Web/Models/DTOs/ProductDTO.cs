using System;
using System.Collections.Generic;
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

public class ProductVariant
{
    [JsonPropertyName("size")]
    public string Size { get; set; } = string.Empty;

    [JsonPropertyName("categories")]
    public string Categories { get; set; } = string.Empty;

    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;

    [JsonPropertyName("variant_id")]
    public string VariantId { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("stockQuantity")]
    public int StockQuantity { get; set; }

    [JsonPropertyName("isFeatured")]
    public bool IsFeatured { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("createdBy")]
    public string? CreatedBy { get; set; }

    [JsonPropertyName("updatedBy")]
    public string? UpdatedBy { get; set; }
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

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("isDelete")]
    public bool IsDelete { get; set; }

    [JsonPropertyName("imageUrls")]
    public List<string> ImageUrls { get; set; } = new();

    [JsonPropertyName("variants")]
    public List<ProductVariant> Variants { get; set; } = new();

    // Helper property to get first image if available
    public string? Image => ImageUrls.FirstOrDefault();

    // Helper property to display category name
    public string? CategoryName => ProductCategoryTitle;

    // Helper property for checking if product is active
    public bool IsActive => Status == 1 && !IsDelete;

    // Helper properties to get first variant's data (for backward compatibility)
    public string? Category => Variants.FirstOrDefault()?.Categories;
    public decimal? Price => Variants.FirstOrDefault()?.Price;
    public string? Size => Variants.FirstOrDefault()?.Size;
    public string? Color => Variants.FirstOrDefault()?.Color;
    public string? VariantId => Variants.FirstOrDefault()?.VariantId;
}