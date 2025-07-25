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
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public List<ProductVariantDTO> Variants { get; set; } = new List<ProductVariantDTO>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Helper method để chuyển đổi URL ảnh thành full URL của Backend API
        public List<string> GetFullImageUrls(string apiBaseUrl)
        {
            return Images.Select(img => GetFullImageUrl(img, apiBaseUrl)).ToList();
        }

        public string GetFirstImageUrl(string apiBaseUrl)
        {
            return Images.Any() ? GetFullImageUrl(Images.First(), apiBaseUrl) : string.Empty;
        }

        private string GetFullImageUrl(string imageUrl, string apiBaseUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return string.Empty;

            // Nếu URL đã là full URL thì trả về nguyên
            if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://"))
                return imageUrl;

            // Nếu là relative path thì thêm base URL của API
            if (imageUrl.StartsWith("/"))
                return $"{apiBaseUrl.TrimEnd('/')}{imageUrl}";

            // Nếu không có / ở đầu thì thêm
            return $"{apiBaseUrl.TrimEnd('/')}/{imageUrl}";
        }
    }

    public class ProductVariantDTO
    {
        public int? VariantId { get; set; }
        public int ProductId { get; set; }
        public string Attributes { get; set; } = string.Empty;
        public List<Dictionary<string, object>> Variants { get; set; } = new List<Dictionary<string, object>>();
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