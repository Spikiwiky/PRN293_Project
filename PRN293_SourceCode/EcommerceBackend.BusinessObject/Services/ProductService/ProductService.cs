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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        private ProductsDto MapToDTO(Product product)
        {
            if (string.IsNullOrEmpty(product.Variants))
            {
                return new ProductsDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductCategoryId = (int)product.ProductCategoryId,
                    ProductCategoryTitle = product.ProductCategory?.ProductCategoryTitle,
                    Description = product.Description,
                    Status = (int)product.Status,
                    IsDelete = (bool)product.IsDelete,
                    ImageUrls = product.ProductImages?.Select(pi => pi.ImageUrl).ToList() ?? new List<string>()
                };
            }

            var variants = JsonDocument.Parse(product.Variants).RootElement;
            return new ProductsDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductCategoryId = (int)product.ProductCategoryId,
                ProductCategoryTitle = product.ProductCategory?.ProductCategoryTitle,
                Description = product.Description,
                Size = variants.TryGetProperty("size", out var sizeProp) ? sizeProp.GetString() : null,
                Color = variants.TryGetProperty("color", out var colorProp) ? colorProp.GetString() : null,
                Category = variants.TryGetProperty("categories", out var categoryProp) ? categoryProp.GetString() : null,
                variant_id = variants.TryGetProperty("variant_id", out var variantIdProp) ? variantIdProp.GetString() : null,
                Price = variants.TryGetProperty("price", out var priceProp) ? priceProp.GetDecimal() : null,
                Status = (int)product.Status,
                IsDelete = (bool)product.IsDelete,
                ImageUrls = product.ProductImages?.Select(pi => pi.ImageUrl).ToList() ?? new List<string>()
            };
        }

        public async Task<List<ProductsDto>> LoadProductsAsync(int page, int pageSize)
        {
            var products = await _productRepository.GetAllProductsAsync(page, pageSize);
            var dtos = products.Select(p => MapToDTO(p)).ToList();
            return dtos.Where(dto => dto.Status == 1 && !dto.IsDelete).ToList();
        }

        public async Task<List<ProductsDto>> SearchProductsAsync(string name = null, string category = null, string size = null, string color = null, string variantId = null, decimal? price = null, int page = 1, int pageSize = 10)
        {
            var products = await _productRepository.SearchProductsAsync(name ?? "", category ?? "", size ?? "", color ?? "", variantId ?? "", price, page, pageSize);
            var dtos = products.Select(p => MapToDTO(p)).ToList();
            return dtos.Where(dto => dto.Status == 1 && !dto.IsDelete).ToList();
        }
    }
}