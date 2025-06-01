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
        Task<List<ProductsDto>> SearchProductsAsync(string name, string category, string size, string color, int page, int pageSize);
    }
}
