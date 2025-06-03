using EcommerceBackend.BusinessObject.dtos.ProductDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceBackend.BusinessObject.dtos;
using EcommerceBackend.BusinessObject.dtos.Shared;

namespace EcommerceBackend.BusinessObject.Services.ProductService
{
    public interface IProductService
    {
        Task<List<ProductsDto>> LoadProductsAsync(int page = 1, int pageSize = 10);
        Task<ProductsDto?> GetProductByIdAsync(int id);
        Task<List<ProductsDto>> SearchProductsAsync(
            string? name = null,
            string? category = null,
            string? size = null,
            string? color = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 10);
        Task<ProductVariant> AddVariantAsync(AddVariantDTO variant);
        Task<int> GetTotalProductCountAsync();
    }
}
