using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.dtos.SaleDto
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ProductCategoryId { get; set; }
        public string Brand { get; set; }
        public decimal BasePrice { get; set; }
        public string AvailableAttributes { get; set; }
        public int? Status { get; set; }
        public bool IsDelete { get; set; }
        public List<ProductImageDto> ProductImages { get; set; }
        public List<ProductVariantDto> Variants { get; set; }
    }

    public class UpdateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ProductCategoryId { get; set; }
        public string Brand { get; set; }
        public decimal BasePrice { get; set; }
        public string AvailableAttributes { get; set; }
        public int? Status { get; set; }
        public bool IsDelete { get; set; }
        public List<ProductImageDto> ProductImages { get; set; }
        public List<ProductVariantDto> Variants { get; set; }
    }

    public class ProductImageDto
    {
        public string ImageUrl { get; set; }
    }

    public class ProductVariantDto
    {
        public string Attributes { get; set; }
        public string Variants { get; set; }
    }

    public class CreateProductCategoryDto
    {
        public string ProductCategoryTitle { get; set; }
        public bool? IsDelete { get; set; } = false;
    }

    public class UpdateProductCategoryDto
    {
        public string ProductCategoryTitle { get; set; }
        public bool? IsDelete { get; set; }
    }

    public class ProductCategoryResponseDto
    {
        public int ProductCategoryId { get; set; }
        public string ProductCategoryTitle { get; set; }
        public bool? IsDelete { get; set; }
    }
}
