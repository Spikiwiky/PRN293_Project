using EcommerceBackend.BusinessObject.Abstract.BlogCategoryAbstract;
using EcommerceBackend.BusinessObject.dtos;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.BlogCategoryService
{
    public class BlogCategoryService : IBlogCategoryService
    {
        private readonly IBlogCategoryRepository _repository;

        public BlogCategoryService(IBlogCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BlogCategoryDto>> GetAllCategoriesAsync(bool includeDeleted = false)
        {
            var categories = await _repository.GetAllAsync(includeDeleted);
            return categories.Select(c => new BlogCategoryDto
            {
                BlogCategoryId = c.BlogCategoryId,
                BlogCategoryTitle = c.BlogCategoryTitle,
                IsDelete = c.IsDelete == true
            }).ToList();
        }

        public async Task<BlogCategoryDto> GetCategoryByIdAsync(int id, bool includeDeleted = false)
        {
            var category = await _repository.GetByIdAsync(id, includeDeleted);
            if (category == null) return null;

            return new BlogCategoryDto
            {
                BlogCategoryId = category.BlogCategoryId,
                BlogCategoryTitle = category.BlogCategoryTitle,
                IsDelete = category.IsDelete == true
            };
        }

        public async Task<BlogCategoryDto> CreateCategoryAsync(CreateBlogCategoryDto dto)
        {
            var category = new BlogCategory
            {
                BlogCategoryTitle = dto.BlogCategoryTitle,
                IsDelete = dto.IsDelete ? true : false
            };

            await _repository.AddAsync(category);

            return new BlogCategoryDto
            {
                BlogCategoryId = category.BlogCategoryId,
                BlogCategoryTitle = category.BlogCategoryTitle,
                IsDelete = category.IsDelete == true
            };
        }

        public async Task UpdateCategoryAsync(UpdateBlogCategoryDto dto)
        {
            var category = await _repository.GetByIdAsync(dto.BlogCategoryId);
            if (category == null)
                throw new KeyNotFoundException("Category not found");

            category.BlogCategoryTitle = dto.BlogCategoryTitle;
            category.IsDelete = dto.IsDelete ? true : false;

            await _repository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(int id, bool hardDelete = false)
        {
            if (hardDelete)
                await _repository.HardDeleteAsync(id);
            else
                await _repository.SoftDeleteAsync(id);
        }

        public async Task RestoreCategoryAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id, includeDeleted: true);
            if (category == null)
                throw new KeyNotFoundException("Category not found");

            category.IsDelete = false;
            await _repository.UpdateAsync(category);
        }
    }
}
