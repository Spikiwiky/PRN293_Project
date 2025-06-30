using System.Text.Json.Serialization;

namespace EcommerceFrontend.Web.Models.Sale
{

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

        public class SaleProductDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int ProductCategoryId { get; set; }
            public string? ProductCategoryTitle { get; set; }
            public List<string> ImageUrls { get; set; } = new();
            public List<ProductVariant> Variants { get; set; } = new();
            public int Status { get; set; }

            [JsonPropertyName("createdAt")]
            public DateTime? CreatedAt { get; set; }

            [JsonPropertyName("updatedAt")]
            public DateTime? UpdatedAt { get; set; }

            [JsonPropertyName("createdBy")]
            public string? CreatedBy { get; set; }

            [JsonPropertyName("updatedBy")]
            public string? UpdatedBy { get; set; }

            // Helper properties to get first variant's data (for backward compatibility)
            public string? Category => Variants.FirstOrDefault()?.Categories;
            public decimal? Price => Variants.FirstOrDefault()?.Price;
            public string? Size => Variants.FirstOrDefault()?.Size;
            public string? Color => Variants.FirstOrDefault()?.Color;
            public bool IsFeatured => Variants.FirstOrDefault()?.IsFeatured ?? false;
            public int StockQuantity => Variants.FirstOrDefault()?.StockQuantity ?? 0;
            public string? VariantId => Variants.FirstOrDefault()?.VariantId;
        }

        public class SaleProductCreateDto
        {
            public string ProductName { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int ProductCategoryId { get; set; }
            public List<string> ImageUrls { get; set; } = new();
            public List<ProductVariant> Variants { get; set; } = new();
            public int Status { get; set; } = 1;

            // For backward compatibility
            public string? Category { get; set; }
            public decimal Price { get; set; }
            public string? Size { get; set; }
            public string? Color { get; set; }
            public bool IsFeatured { get; set; }
            public int StockQuantity { get; set; }
            public string? VariantId { get; set; }
            public string? CreatedBy { get; set; }
        }

        public class SaleProductUpdateDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int? ProductCategoryId { get; set; }
            public List<string>? ImageUrls { get; set; }
            public List<ProductVariant>? Variants { get; set; }
            public int? Status { get; set; }

            // For backward compatibility
            public string? Category { get; set; }
            public decimal? Price { get; set; }
            public string? Size { get; set; }
            public string? Color { get; set; }
            public bool? IsFeatured { get; set; }
            public int? StockQuantity { get; set; }
            public string? VariantId { get; set; }
            public string? UpdatedBy { get; set; }
        }
    }
