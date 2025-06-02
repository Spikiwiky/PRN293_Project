using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.dtos.UserDto
{
    public class UserDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Phone { get; set; }
        public string UserName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; }

        public DateTime CreateDate { get; set; }

        public int Status { get; set; }

        public bool IsDelete { get; set; }

        // public object Role { get; set; }
        // public List<object> Carts { get; set; }
        // public List<object> Orders { get; set; }
    }

    //public class UserDto
    //{
    //    [JsonPropertyName("userId")]
    //    public int UserId { get; set; }

    //    [JsonPropertyName("roleId")]
    //    public int RoleId { get; set; }

    //    [JsonPropertyName("email")]
    //    [Required]
    //    [EmailAddress]
    //    public string Email { get; set; }

    //    [JsonPropertyName("password")]
    //    [Required]
    //    public string Password { get; set; }

    //    [JsonPropertyName("phone")]
    //    public string Phone { get; set; }

    //    [JsonPropertyName("userName")]
    //    public string UserName { get; set; }

    //    [JsonPropertyName("dateOfBirth")]
    //    public DateTime? DateOfBirth { get; set; }

    //    [JsonPropertyName("address")]
    //    public string Address { get; set; }

    //    [JsonPropertyName("createDate")]
    //    public DateTime CreateDate { get; set; }

    //    [JsonPropertyName("status")]
    //    public int Status { get; set; }

    //    [JsonPropertyName("isDelete")]
    //    public bool IsDelete { get; set; }

    //    [JsonPropertyName("role")]
    //    public RoleDto Role { get; set; }

    //    [JsonPropertyName("carts")]
    //    public List<CartDto> Carts { get; set; }

    //    [JsonPropertyName("orders")]
    //    public List<OrderDto> Orders { get; set; }
    //}

    //public class RoleDto
    //{
    //    [JsonPropertyName("roleId")]
    //    public int RoleId { get; set; }

    //    [JsonPropertyName("name")]
    //    public string Name { get; set; }

    //    [JsonPropertyName("description")]
    //    public string Description { get; set; }
    //}

    //public class CartDto
    //{
    //    [JsonPropertyName("cartId")]
    //    public int CartId { get; set; }

    //    [JsonPropertyName("userId")]
    //    public int UserId { get; set; }

    //    [JsonPropertyName("createdAt")]
    //    public DateTime CreatedAt { get; set; }

    //    [JsonPropertyName("cartItems")]
    //    public List<CartItemDto> CartItems { get; set; }
    //}

    //public class CartItemDto
    //{
    //    [JsonPropertyName("cartItemId")]
    //    public int CartItemId { get; set; }

    //    [JsonPropertyName("cartId")]
    //    public int CartId { get; set; }

    //    [JsonPropertyName("variantId")]
    //    public int VariantId { get; set; }

    //    [JsonPropertyName("quantity")]
    //    public int Quantity { get; set; }
    //}

    //public class OrderDto
    //{
    //    [JsonPropertyName("orderId")]
    //    public int OrderId { get; set; }

    //    [JsonPropertyName("userId")]
    //    public int UserId { get; set; }

    //    [JsonPropertyName("orderDate")]
    //    public DateTime OrderDate { get; set; }

    //    [JsonPropertyName("totalAmount")]
    //    public decimal TotalAmount { get; set; }

    //    [JsonPropertyName("status")]
    //    public string Status { get; set; }

    //    [JsonPropertyName("shippingAddress")]
    //    public string ShippingAddress { get; set; }
    //}
}
