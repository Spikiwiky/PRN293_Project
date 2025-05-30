using System;
using System.Collections.Generic;

namespace EcommerceBackend.DataAccess.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? AmountDue { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? OrderStatusId { get; set; }
        public string? OrderNote { get; set; }

        public virtual User? Customer { get; set; }
        public virtual OrderStatus? OrderStatus { get; set; }
        public virtual PaymentMethod? PaymentMethod { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
