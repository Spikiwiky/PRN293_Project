namespace EcommerceFrontend.Web.Models.Sale
{
    public class CreateProductInputModel
    {
        // Product basic info
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ProductCategoryId { get; set; }
        public string Brand { get; set; }
        public decimal BasePrice { get; set; }
        public int? Status { get; set; }
        public bool IsDelete { get; set; }
        public string ImageUrl { get; set; }
         
        public List<string> AvailableSizes { get; set; } = new List<string>();
        public List<string> AvailableColors { get; set; } = new List<string>();
         
        public List<ProductVariantInputModel> Variants { get; set; } = new List<ProductVariantInputModel>();
    }

    public class ProductVariantInputModel
    {
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }


}
