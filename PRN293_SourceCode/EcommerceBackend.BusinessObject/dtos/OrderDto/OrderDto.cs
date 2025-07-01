using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.dtos.OrderDto
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? AmountDue { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OrderStatus { get; set; }
        public string? OrderNote { get; set; }

        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }
}
