using EcommerceBackend.BusinessObject.dtos.ProductDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceBackend.BusinessObject.dtos;
namespace EcommerceBackend.BusinessObject.Services.ProductService
{
    public interface IProductService
    {
        Task<List<ProductsDto>> LoadProductsAsync(int page, int pageSize);
        Task<List<ProductsDto>> SearchProductsAsync(
            string? name = null,
            string? category = null,
            string? size = null,
            string? color = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 10);
    }
}
