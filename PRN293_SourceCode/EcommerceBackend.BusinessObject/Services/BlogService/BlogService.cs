using EcommerceBackend.BusinessObject.Abstract.BlogAbstract;
using EcommerceBackend.BusinessObject.dtos.BlogDto;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Abstract.BlogAbstract;
using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.DataAccess.Repository.BlogRepository;
using System.Net.Http;

namespace EcommerceBackend.BusinessObject.Services
{
    public class BlogService : IBlogService
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

        public async Task<BlogDto> GetByIdAsync(int id)
        {
            var b = await _repository.GetByIdAsync(id);
            return new BlogDto
            {
                BlogId = b.BlogId,
                BlogCategoryId = b.BlogCategoryId,
                BlogTittle = b.BlogTittle,
                BlogContent = b.BlogContent
            };
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
        public async Task<IEnumerable<BlogDto>> LoadBlogsAsync(int page, int pageSize)
        {
            var blogs = await _repository.GetPagedAsync(page, pageSize);
            return blogs.Select(b => new BlogDto
            {
                BlogId = b.BlogId,
                BlogCategoryId = b.BlogCategoryId,
                BlogTittle = b.BlogTittle,
                BlogContent = b.BlogContent
            });
        }

        //public async Task<List<BlogDto>> LoadBlogsAsync()
        //{
        //    var response = await _repository.GetFromJsonAsync<List<BlogDto>>("api/blog/load?page=1&pageSize=10");
        //    return response ?? new List<BlogDto>();
        //}
        //public async Task<PagedResponse<BlogDto>> LoadBlogsAsync(int page, int pageSize)
        //{
        //    var query = await _blogRepository.GetAllAsync();

        //    var totalItems = query.Count;
        //    var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        //    var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //    return new PagedResponse<BlogDto>
        //    {
        //        Items = items.Select(b => new BlogDto
        //        {
        //            BlogId = b.BlogId,
        //            Title = b.Title,
        //            Description = b.Description,
        //            Image = b.Image,
        //            CreatedAt = b.CreatedAt,
        //            Status = b.Status
        //        }).ToList(),
        //        TotalItems = totalItems,
        //        TotalPages = totalPages,
        //        CurrentPage = page
        //    };
        //}

    }
}
