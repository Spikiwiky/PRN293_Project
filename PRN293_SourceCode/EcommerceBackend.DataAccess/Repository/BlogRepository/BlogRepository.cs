using EcommerceBackend.DataAccess.Abstract.BlogAbstract;
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly EcommerceDBContext _context;

        public BlogRepository(EcommerceDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Blog>> GetAllAsync(bool includeDeleted = false)
        {
            var query = _context.Blogs
                .Include(b => b.BlogCategory)
                .Include(b => b.Comments.Where(c => includeDeleted || !c.IsDeleted))
                .AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(b => !b.IsDelete);
            }

            return await query.ToListAsync();
        }

        public async Task<Blog> GetByIdAsync(int id, bool includeDeleted = false)
        {
            var query = _context.Blogs
                .Include(b => b.BlogCategory)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.User)
                .AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(b => !b.IsDelete)
                             .Select(b => new Blog
                             {
                                 BlogId = b.BlogId,
                                 BlogTittle = b.BlogTittle,
                                 BlogContent = b.BlogContent,
                                 BlogCategoryId = b.BlogCategoryId,
                                 CreatedAt = b.CreatedAt,
                                 IsDelete = b.IsDelete,
                                 BlogCategory = b.BlogCategory,
                                 Comments = b.Comments.Where(c => !c.IsDeleted).ToList()
                             });
            }

            return await query.FirstOrDefaultAsync(b => b.BlogId == id);
        }

        public async Task HardDeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // First delete all comments
                var comments = await _context.Comments
                    .Where(c => c.BlogId == id)
                    .ToListAsync();

                _context.Comments.RemoveRange(comments);

                // Then delete the blog
                var blog = await _context.Blogs.FindAsync(id);
                if (blog != null)
                {
                    _context.Blogs.Remove(blog);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddAsync(Blog blog)
        {
            blog.CreatedAt = DateTime.UtcNow;
            blog.IsDelete = false;
            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Blog blog)
        {
            blog.IsDelete = false; // Prevent accidental deletion via update
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await HardDeleteAsync(id); // Legacy method defaults to hard delete
        }

        public async Task SoftDeleteAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                blog.IsDelete = true;
                await _context.SaveChangesAsync();
            }
        }

      

        public async Task RestoreAsync(int id)
        {
            var blog = await _context.Blogs
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.BlogId == id && b.IsDelete);

            if (blog != null)
            {
                blog.IsDelete = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Blog>> GetPagedAsync(int page, int pageSize, bool includeDeleted = false)
        {
            var query = _context.Blogs
                .Include(b => b.BlogCategory)
                .OrderByDescending(b => b.CreatedAt)
                .AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(b => !b.IsDelete);
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Blog>> FindByTitleAsync(string title, bool includeDeleted = false)
        {
            var query = _context.Blogs
                .Include(b => b.BlogCategory)
                .Where(b => b.BlogTittle.Contains(title))
                .AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(b => !b.IsDelete);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Blog>> GetByCategoryIdAsync(int categoryId, bool includeDeleted = false)
        {
            var query = _context.Blogs
                .Include(b => b.BlogCategory)
                .Where(b => b.BlogCategoryId == categoryId)
                .AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(b => !b.IsDelete);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> ExistsAsync(int blogId)
        {
            return await _context.Blogs
                .AnyAsync(b => b.BlogId == blogId && !b.IsDelete);
        }

        public async Task<bool> ExistsAsync(int blogId, bool includeDeleted = false)
        {
            var query = _context.Blogs.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(b => !b.IsDelete);
            }

            return await query.AnyAsync(b => b.BlogId == blogId);
        }

        public async Task<IEnumerable<BlogCategory>> GetAllCategoriesAsync()
        {
            return await _context.BlogCategories
                .Where(c => !c.IsDelete)
                .ToListAsync();
        }

        public async Task<Blog> CreateAsync(Blog blog, int categoryId)
        {
            blog.BlogCategoryId = categoryId;
            await AddAsync(blog);
            return blog;
        }
    }
}