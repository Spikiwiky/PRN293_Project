using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository.SaleRepository.BlogRepo
{
    public interface ISaleBlogRepository
    {
        Task<IEnumerable<Blog>> GetAllBlogsAsync();
        Task<Blog?> GetBlogByIdAsync(int id);
        Task AddBlogAsync(Blog blog);
        Task UpdateBlogAsync(Blog blog);
        Task DeleteBlogAsync(int id);

        Task<IEnumerable<BlogCategory>> GetAllCategoriesAsync();
        Task<BlogCategory?> GetCategoryByIdAsync(int id);

        Task<IEnumerable<BlogComment>> GetCommentsByBlogIdAsync(int blogId);
        Task AddCommentAsync(BlogComment comment);
        Task DeleteCommentAsync(int commentId);
        Task SaveChangesAsync();
        Task UpdateCommentAsync(BlogComment comment);
        Task<BlogCategory> AddCategoryAsync(BlogCategory category);
        Task UpdateCategoryAsync(BlogCategory category);
        Task DeleteCategoryAsync(BlogCategory category);

    }
}
