using System;
using System.Collections.Generic;

namespace EcommerceBackend.DataAccess.Models
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            Products = new HashSet<Product>();
        }

        public int ProductCategoryId { get; set; }
        public string? ProductCategoryTitle { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
