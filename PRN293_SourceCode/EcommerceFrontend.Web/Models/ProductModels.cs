using System.Text.Json.Serialization;

namespace EcommerceFrontend.Web.Models
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string AvailableAttributes { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public List<ProductVariantDTO> Variants { get; set; } = new List<ProductVariantDTO>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ProductVariantDTO
    {
        public int? VariantId { get; set; }
        public int ProductId { get; set; }
        public string Attributes { get; set; } = string.Empty;
        public List<Dictionary<string, string>> Variants { get; set; } = new List<Dictionary<string, string>>();
    }

    public class ProductSearchParams
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public Dictionary<string, string>? Attributes { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
} 