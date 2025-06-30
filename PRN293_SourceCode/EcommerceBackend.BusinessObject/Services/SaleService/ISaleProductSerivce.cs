using EcommerceBackend.BusinessObject.dtos.SaleDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.SaleService
{
    public interface ISaleProductService
    {
        Task<List<SaleProductDto>> GetAllProductsAsync(int page = 1, int pageSize = 10);
        Task<SaleProductDto?> GetProductByIdAsync(int id);
        Task<List<SaleProductDto>> SearchProductsAsync(
            string? name = null,
            string? category = null,
            string? size = null,
            string? color = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? isFeatured = null,
            int page = 1,
            int pageSize = 10);
        Task<SaleProductDto> CreateProductAsync(SaleProductCreateDto createDto);
        Task<SaleProductDto> UpdateProductAsync(SaleProductUpdateDto updateDto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> UpdateProductFeaturedStatusAsync(int id, bool isFeatured);
        Task<bool> UpdateProductStatusAsync(int id, int status);
        Task<int> GetTotalProductCountAsync();
        Task<List<CategoryDto>> GetCategoriesAsync();
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

