using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceBackend.DataAccess.Models;
public interface IProductRepository
{
    Task<List<Product>> GetAllProductsAsync(int page, int pageSize);
    Task<List<Product>> SearchProductsAsync(string name, string category, string size, string color, int page, int pageSize);
}
