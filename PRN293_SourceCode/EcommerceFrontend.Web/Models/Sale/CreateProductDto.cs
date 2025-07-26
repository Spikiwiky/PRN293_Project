namespace EcommerceFrontend.Web.Models.Sale
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
        public bool IsDelete { get; set; } = false;
        public string ImageUrl { get; set; } = null!;
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
        public int ProductImageId { get; set; }
        public int? ProductId { get; set; }
        public string ImageUrl { get; set; } = null!;

    }
    public class ProductVariantDto
    {
        public int VariantId { get; set; }
        public string Attributes { get; set; }
        public string Variants { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Material { get; set; }
        public string Supplier { get; set; }
        public string XuatXu { get; set; }   // Xuất Xứ
    }

}
