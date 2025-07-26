using System.ComponentModel.DataAnnotations;

namespace EcommerceBackend.BusinessObject.dtos.CartDto
{
    // Request DTOs for Business Logic
    public class AddToCartRequestDto
    {
        [Required]
        public int ProductId { get; set; }
        
        public int? VariantId { get; set; }
        
        public string? VariantAttributes { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }

    public class UpdateCartItemRequestDto
    {
        [Required]
        public int CartDetailId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }

    public class IncreaseCartItemRequestDto
    {
        [Required]
        public int CartDetailId { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantity to add must be greater than 0")]
        public int QuantityToAdd { get; set; } = 1;
    }

    public class DecreaseCartItemRequestDto
    {
        [Required]
        public int CartDetailId { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantity to remove must be greater than 0")]
        public int QuantityToRemove { get; set; } = 1;
    }

    public class RemoveFromCartRequestDto
    {
        [Required]
        public int CartDetailId { get; set; }
    }

    // Response DTOs for Business Logic
    public class CartDetailResponseDto
    {
        public int CartDetailId { get; set; }
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? VariantAttributes { get; set; }
        
        // Product information
        public string? ProductImage { get; set; }
        public string? ProductDescription { get; set; }
    }

    public class CartResponseDto
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public int TotalQuantity { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<CartDetailResponseDto> CartDetails { get; set; } = new List<CartDetailResponseDto>();
    }

    public class CartSummaryResponseDto
    {
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        public int CartItemCount { get; set; }
    }

    public class CartOperationResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public CartResponseDto? Cart { get; set; }
        public CartSummaryResponseDto? Summary { get; set; }
    }
} 