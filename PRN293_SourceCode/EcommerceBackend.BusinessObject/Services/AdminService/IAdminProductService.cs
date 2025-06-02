using EcommerceBackend.BusinessObject.dtos.AdminDto;

namespace EcommerceBackend.BusinessObject.Services.AdminService
{
    public interface IAdminProductService
    {
        Task<List<AdminProductDto>> GetAllProductsAsync(int page = 1, int pageSize = 10);
        Task<AdminProductDto?> GetProductByIdAsync(int id);
        Task<AdminProductDto> CreateProductAsync(AdminProductCreateDto createDto);
        Task<AdminProductDto> UpdateProductAsync(AdminProductUpdateDto updateDto);
        Task<bool> DeleteProductAsync(int id);
        Task<List<AdminProductDto>> SearchProductsAsync(
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
        Task<bool> UpdateProductStatusAsync(int id, int status);
        Task<bool> UpdateProductFeaturedStatusAsync(int id, bool isFeatured);
        Task<int> GetTotalProductCountAsync();
    }
} 