using System;
using System.Collections.Generic;

namespace EcommerceBackend.DataAccess.Models
{
    public partial class Cart
    {
        public Cart()
        {
            CartDetails = new HashSet<CartDetail>();
        }

        public int CartId { get; set; }
        public int? CustomerId { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? AmountDue { get; set; }

        public virtual User? Customer { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
    }
}
