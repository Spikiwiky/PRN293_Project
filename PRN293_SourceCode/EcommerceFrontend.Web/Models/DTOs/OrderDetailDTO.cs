using System;

namespace EcommerceFrontend.Web.Models.DTOs
{
    public class OrderDetailDTO
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public int? VariantId { get; set; }
        public string? VariantAttributes { get; set; }
        public string? ProductImage { get; set; }
    }
} 