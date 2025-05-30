using System;
using System.Collections.Generic;

namespace EcommerceBackend.DataAccess.Models
{
    public partial class Product
    {
        public Product()
        {
            CartDetails = new HashSet<CartDetail>();
            OrderDetails = new HashSet<OrderDetail>();
            ProductImages = new HashSet<ProductImage>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int? ProductCategoryId { get; set; }
        public string? Description { get; set; }
        public string? Variants { get; set; }
        public int? Status { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ProductCategory? ProductCategory { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}
