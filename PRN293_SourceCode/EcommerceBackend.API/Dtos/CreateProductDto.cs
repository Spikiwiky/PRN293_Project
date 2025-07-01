using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EcommerceBackend.BusinessObject.dtos.SaleDto;
public class CreateProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ProductCategoryId { get; set; }
    public string Brand { get; set; }
    public decimal BasePrice { get; set; }

    public string AvailableAttributes { get; set; }

    public int? Status { get; set; }
    public bool IsDelete { get; set; } = false;

    public List<ProductImageDto> ProductImages { get; set; }
    public List<ProductVariantDto> Variants { get; set; }
}

public class UpdateProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProductCategoryId { get; set; }
    public string Brand { get; set; }
    public decimal BasePrice { get; set; }
    public string AvailableAttributes { get; set; }
    public int Status { get; set; }
    public bool IsDelete { get; set; }
    public List<ProductImageDto> ProductImages { get; set; }
    public List<ProductVariantDto> Variants { get; set; }
}


public class ProductResponseDto
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ProductCategoryId { get; set; }
    public string Brand { get; set; }
    public decimal BasePrice { get; set; }
    public string AvailableAttributes { get; set; }
    public int? Status { get; set; }
    public bool IsDelete { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
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
