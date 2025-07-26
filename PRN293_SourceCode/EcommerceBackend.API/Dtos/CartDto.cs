using System.ComponentModel.DataAnnotations;

namespace EcommerceBackend.API.Dtos
{
    // Request DTOs for API
    public class AddToCartDto
    {
        [Required]
        public int ProductId { get; set; }
        
        public int? VariantId { get; set; }
        
        public string? VariantAttributes { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }

    public class UpdateCartItemDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int CartDetailId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }

    public class IncreaseCartItemDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int CartDetailId { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantity to add must be greater than 0")]
        public int QuantityToAdd { get; set; } = 1;
    }

    public class DecreaseCartItemDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int CartDetailId { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantity to remove must be greater than 0")]
        public int QuantityToRemove { get; set; } = 1;
    }

    public class IncreaseCartItemByProductDto
    {
        [Required]
        public int ProductId { get; set; }
        
        public int? VariantId { get; set; }
        
        public string? VariantAttributes { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantity to add must be greater than 0")]
        public int QuantityToAdd { get; set; } = 1;
    }

    public class DecreaseCartItemByProductDto
    {
        [Required]
        public int ProductId { get; set; }
        
        public int? VariantId { get; set; }
        
        public string? VariantAttributes { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantity to remove must be greater than 0")]
        public int QuantityToRemove { get; set; } = 1;
    }

    public class RemoveFromCartDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int CartDetailId { get; set; }
    }

    public class UpdateCartItemByProductDto
    {
        [Required]
        public int ProductId { get; set; }
        
        public int? VariantId { get; set; }
        
        public string? VariantAttributes { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }

    public class RemoveFromCartByProductDto
    {
        [Required]
        public int ProductId { get; set; }
        
        public int? VariantId { get; set; }
        
        public string? VariantAttributes { get; set; }
    }

    // Response DTOs for API
    public class CartDetailDto
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

    public class CartDto
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public int TotalQuantity { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<CartDetailDto> CartDetails { get; set; } = new List<CartDetailDto>();
    }

    public class CartSummaryDto
    {
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        public int CartItemCount { get; set; }
    }

    public class CartResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public CartDto? Cart { get; set; }
        public CartSummaryDto? Summary { get; set; }
    }
} 