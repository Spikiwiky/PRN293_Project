using EcommerceFrontend.Web.Models.Admin;

namespace EcommerceFrontend.Web.Services.Admin
{
    public interface IAdminProductService
    {
        Task<List<AdminProductDto>> GetAllProductsAsync(int page = 1, int pageSize = 10);
        Task<AdminProductDto?> GetProductByIdAsync(int id);
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
        Task<AdminProductDto> CreateProductAsync(AdminProductCreateDto createDto);
        Task<AdminProductDto> UpdateProductAsync(AdminProductUpdateDto updateDto);
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