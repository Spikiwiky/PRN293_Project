using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.DTOs
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string AvailableAttributes { get; set; } = "{}";
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public List<ProductVariantDTO> Variants { get; set; } = new List<ProductVariantDTO>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Status { get; set; }
        public bool IsDelete { get; set; }
    }

    public class ProductVariantDTO
    {
        public int? VariantId { get; set; }
        public int ProductId { get; set; }
        public string Attributes { get; set; } = string.Empty;
        public List<Dictionary<string, object>> Variants { get; set; } = new List<Dictionary<string, object>>();
    }
}
