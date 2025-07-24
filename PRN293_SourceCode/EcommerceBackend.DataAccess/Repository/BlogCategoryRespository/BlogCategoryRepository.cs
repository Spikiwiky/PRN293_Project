using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository.BlogCategoryRespository
{
    public class BlogCategoryRepository : IBlogCategoryRepository
    {
        private readonly EcommerceDBContext _context;

        public BlogCategoryRepository(EcommerceDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlogCategory>> GetAllAsync(bool includeDeleted = false)
        {
            var query = _context.BlogCategories.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(c => c.IsDelete == false);
            }

            return await query.ToListAsync();
        }

        public async Task<BlogCategory> GetByIdAsync(int id)
        {
            return await _context.BlogCategories
                .FirstOrDefaultAsync(c => c.BlogCategoryId == id && c.IsDelete == false);
        }

        public async Task<BlogCategory> GetByIdAsync(int id, bool includeDeleted = false)
        {
            var query = _context.BlogCategories.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(c => c.IsDelete == false);
            }

            return await query.FirstOrDefaultAsync(c => c.BlogCategoryId == id);
        }

        public async Task AddAsync(BlogCategory category)
        {
            await _context.BlogCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BlogCategory category)
        {
            _context.BlogCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var category = await _context.BlogCategories.FindAsync(id);
            if (category != null)
            {
                category.IsDelete = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RestoreAsync(int id)
        {
            var category = await _context.BlogCategories
                .FirstOrDefaultAsync(c => c.BlogCategoryId == id && c.IsDelete == true);

            if (category != null)
            {
                category.IsDelete = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task HardDeleteAsync(int id)
        {
            var category = await _context.BlogCategories
                .Include(c => c.Blogs)
                .FirstOrDefaultAsync(c => c.BlogCategoryId == id);

            if (category != null)
            {
                foreach (var blog in category.Blogs)
                {
                    blog.BlogCategoryId = null;
                }

                _context.BlogCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.BlogCategories
                .AnyAsync(c => c.BlogCategoryId == id && c.IsDelete == false);
        }

        public async Task<bool> ExistsAsync(int id, bool includeDeleted = false)
        {
            var query = _context.BlogCategories.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(c => c.IsDelete == false);
            }

            return await query.AnyAsync(c => c.BlogCategoryId == id);
        }
    }
}
