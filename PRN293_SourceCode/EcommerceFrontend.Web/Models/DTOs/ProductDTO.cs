using System;

namespace EcommerceFrontend.Web.Models.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public bool IsDelete { get; set; }
    }
} 