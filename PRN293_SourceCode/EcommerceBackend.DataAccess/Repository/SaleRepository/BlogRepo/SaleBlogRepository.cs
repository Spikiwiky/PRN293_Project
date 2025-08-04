using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository.SaleRepository.BlogRepo
{
    public class SaleBlogRepository : ISaleBlogRepository
    {
        private readonly EcommerceDBContext _context;

        public SaleBlogRepository(EcommerceDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Blog>> GetAllBlogsAsync()
        {
            return await _context.Blogs
                .Where(b => b.IsPublished == true)   // loại bỏ IsDelete = true
                .Include(b => b.BlogCategory)
                .Include(b => b.Comments)
                .ToListAsync();
        }

        public async Task<Blog?> GetBlogByIdAsync(int id)
        {
            return await _context.Blogs
                .Where(b => b.BlogId == id && b.IsPublished == true)  
                .Include(b => b.BlogCategory)
                .Include(b => b.Comments)
                .FirstOrDefaultAsync();
        }


        public async Task AddBlogAsync(Blog blog)
        {
            await _context.Blogs.AddAsync(blog);
        }

        public async Task UpdateBlogAsync(Blog blog)
        {
            _context.Blogs.Update(blog);
        }

        public async Task DeleteBlogAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                blog.IsPublished = false;
                _context.Blogs.Update(blog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<BlogCategory>> GetAllCategoriesAsync()
        {
            return await _context.BlogCategories
                .Where(c => c.IsDelete == false) 
                .ToListAsync();
        }

        public async Task<BlogCategory?> GetCategoryByIdAsync(int id)
        {
            return await _context.BlogCategories
                .FirstOrDefaultAsync(c => c.BlogCategoryId == id && (c.IsDelete == false));
        }


        public async Task<IEnumerable<BlogComment>> GetCommentsByBlogIdAsync(int blogId)
        {
            return await _context.BlogComments
                .Where(c => c.BlogId == blogId)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task AddCommentAsync(BlogComment comment)
        {
            await _context.BlogComments.AddAsync(comment);
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await _context.BlogComments.FindAsync(commentId);
            if (comment != null)
            {
                _context.BlogComments.Remove(comment);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCommentAsync(BlogComment comment)
        {
            _context.BlogComments.Update(comment);
        }
        public async Task<BlogCategory> AddCategoryAsync(BlogCategory category)
        {
            _context.BlogCategories.Add(category);
            await SaveChangesAsync();
            return category;
        }

        public async Task UpdateCategoryAsync(BlogCategory category)
        {
            _context.BlogCategories.Update(category);
            await SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(BlogCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Blog category cannot be null for deletion.");
            }

            category.IsDelete = true;

            _context.BlogCategories.Update(category); 

            await SaveChangesAsync();
        }
    }
}

