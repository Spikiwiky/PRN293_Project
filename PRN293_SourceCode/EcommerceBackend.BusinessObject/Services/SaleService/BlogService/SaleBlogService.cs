using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.DataAccess.Repository.SaleRepository.BlogRepo;

namespace EcommerceBackend.BusinessObject.Services.SaleService.BlogService
{
    public class SaleBlogService : ISaleBlogService
    {
        private readonly ISaleBlogRepository _repository;

        public SaleBlogService(ISaleBlogRepository repository)
        {
            _repository = repository;
        }

        // Blog
        public async Task<IEnumerable<Blog>> GetAllBlogsAsync() => await _repository.GetAllBlogsAsync();

        public async Task<Blog?> GetBlogByIdAsync(int id) => await _repository.GetBlogByIdAsync(id);

        public async Task<Blog> CreateBlogAsync(Blog blog)
        {
            blog.CreatedDate = DateTime.UtcNow;
            await _repository.AddBlogAsync(blog);
            await _repository.SaveChangesAsync();
            return blog;
        }

        public async Task UpdateBlogAsync(Blog blog)
        {
            blog.UpdatedDate = DateTime.UtcNow;
            await _repository.UpdateBlogAsync(blog);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteBlogAsync(int id)
        {
            await _repository.DeleteBlogAsync(id);
            await _repository.SaveChangesAsync();
        }

        // Category
        public async Task<IEnumerable<BlogCategory>> GetAllCategoriesAsync() => await _repository.GetAllCategoriesAsync();

        public async Task<BlogCategory?> GetCategoryByIdAsync(int id) => await _repository.GetCategoryByIdAsync(id);

        public async Task<BlogCategory> CreateCategoryAsync(BlogCategory category)
        {
            var created = await _repository.AddCategoryAsync(category);
            await _repository.SaveChangesAsync();
            return created;
        }

        public async Task<BlogCategory?> UpdateCategoryAsync(BlogCategory category)
        {
            var existing = await _repository.GetCategoryByIdAsync(category.BlogCategoryId);
            if (existing == null) return null;

            existing.BlogCategoryTitle = category.BlogCategoryTitle;
            await _repository.UpdateCategoryAsync(existing);
            await _repository.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _repository.GetCategoryByIdAsync(id);
            if (category == null) return false;

            await _repository.DeleteCategoryAsync(category);
            await _repository.SaveChangesAsync();
            return true;
        }

        // Comment
        public async Task<IEnumerable<BlogComment>> GetCommentsByBlogIdAsync(int blogId)
            => await _repository.GetCommentsByBlogIdAsync(blogId);

        public async Task<BlogComment> AddCommentAsync(BlogComment comment)
        {
            comment.CreatedDate = DateTime.UtcNow;
            await _repository.AddCommentAsync(comment);
            await _repository.SaveChangesAsync();
            return comment;
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            await _repository.DeleteCommentAsync(commentId);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateCommentAsync(BlogComment comment)
        {
            await _repository.UpdateCommentAsync(comment);
            await _repository.SaveChangesAsync();
        }
    }
}
