namespace EcommerceBackend.BusinessObject.dtos.CartDto
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string VariantAttributes { get; set; }
        public int CartDetailId { get; set; }
    }
} 