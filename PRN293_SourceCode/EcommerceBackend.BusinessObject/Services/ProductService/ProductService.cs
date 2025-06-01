using EcommerceBackend.BusinessObject.dtos.ProductDto;
using EcommerceBackend.DataAccess;
using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace EcommerceBackend.BusinessObject.Services.ProductService
{
    public class ProductService: IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        private ProductsDto MapToDTO(Product product)
        {
            var variants = JsonDocument.Parse(product.Variants).RootElement;
            return new ProductsDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductCategoryId = (int)product.ProductCategoryId,
                ProductCategoryTitle = product.ProductCategory.ProductCategoryTitle,
                Description = product.Description,
                Size = variants.GetProperty("size").GetString(),
                Color = variants.GetProperty("color").GetString(),
                Category = variants.GetProperty("categories").GetString(),
                Status = (int)product.Status,
                IsDelete = (bool)product.IsDelete,
                ImageUrls = product.ProductImages.Select(pi => pi.ImageUrl).ToList()
            };
        }


        public async Task<List<ProductsDto>> LoadProductsAsync(int page, int pageSize)
        {
            var products = await _productRepository.GetAllProductsAsync(page, pageSize);
            var dtos = products.Select(p => MapToDTO(p)).ToList();
            return dtos.Where(dto => dto.Status == 1 && !dto.IsDelete).ToList(); 
        }

        public async Task<List<ProductsDto>> SearchProductsAsync(string name, string category, string size, string color, int page, int pageSize)
        {
            var products = await _productRepository.SearchProductsAsync(name ?? "", category ?? "", size ?? "", color ?? "", page, pageSize);
            var dtos = products.Select(p => MapToDTO(p)).ToList();
            return dtos.Where(dto => dto.Status == 1 && !dto.IsDelete).ToList();
        }
    }
}
