using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.DataAccess.Repository.SaleRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.SaleService
{
    public class SaleService : ISaleService
    {
        private readonly IProductRepository _productRepository;

        public SaleService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task CreateProductAsync(Product product)
        {
            await _productRepository.AddProductAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteProductAsync(id);
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }
    }
}
