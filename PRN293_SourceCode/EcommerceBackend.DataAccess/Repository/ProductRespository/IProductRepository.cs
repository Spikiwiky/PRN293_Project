using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceBackend.DataAccess.Models;

namespace EcommerceBackend.DataAccess.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync(int page, int pageSize);
        Task<List<Product>> SearchProductsAsync(
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
