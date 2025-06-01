using EcommerceBackend.BusinessObject.dtos.BlogDto;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Abstract.BlogAbstract;
using EcommerceBackend.DataAccess.Models;

namespace EcommerceBackend.BusinessObject.Services
{
    public class BlogService
    {
        private readonly IBlogRepository _repository;

        public BlogService(IBlogRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BlogDto>> GetAllAsync()
        {
            var blogs = await _repository.GetAllAsync();
            return blogs.Select(b => new BlogDto
            {
                BlogId = b.BlogId,
                BlogCategoryId = b.BlogCategoryId,
                BlogTittle = b.BlogTittle,
                BlogContent = b.BlogContent
            });
        }

        public async Task<BlogDto?> GetByIdAsync(int id)
        {
            var b = await _repository.GetByIdAsync(id);
            return b == null ? null : new BlogDto
            {
                BlogId = b.BlogId,
                BlogCategoryId = b.BlogCategoryId,
                BlogTittle = b.BlogTittle,
                BlogContent = b.BlogContent
            };
        }

        public async Task AddAsync(BlogDto dto)
        {
            var blog = new Blog
            {
                BlogCategoryId = dto.BlogCategoryId,
                BlogTittle = dto.BlogTittle,
                BlogContent = dto.BlogContent
            };
            await _repository.AddAsync(blog);
        }

        public async Task UpdateAsync(BlogDto dto)
        {
            var blog = new Blog
            {
                BlogId = dto.BlogId,
                BlogCategoryId = dto.BlogCategoryId,
                BlogTittle = dto.BlogTittle,
                BlogContent = dto.BlogContent
            };
            await _repository.UpdateAsync(blog);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
