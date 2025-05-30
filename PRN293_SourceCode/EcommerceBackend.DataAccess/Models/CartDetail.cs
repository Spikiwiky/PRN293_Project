using System;
using System.Collections.Generic;

namespace EcommerceBackend.DataAccess.Models
{
    public partial class CartDetail
    {
        public int CartDetailId { get; set; }
        public int? CartId { get; set; }
        public int? ProductId { get; set; }
        public string? VariantId { get; set; }
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }

        public virtual Cart? Cart { get; set; }
        public virtual Product? Product { get; set; }
    }
}
