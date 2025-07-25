using EcommerceBackend.DataAccess.Abstract.BlogAbstract;
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository.BlogRepository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly EcommerceDBContext _context;

        public BlogRepository(EcommerceDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Blog>> GetAllAsync() =>
            await _context.Blogs.Include(b => b.BlogCategory).Include(b => b.User).ToListAsync();

        public async Task<Blog?> GetByIdAsync(int id) =>
            await _context.Blogs.Include(b => b.BlogCategory).Include(b => b.User).FirstOrDefaultAsync(b => b.BlogId == id);

        public async Task AddAsync(Blog blog)
        {
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Blog blog)
        {
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var blog = await GetByIdAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
            }
        }

        public Task<Blog> CreateAsync(Blog blog)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Blog>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.Blogs
                .Include(b => b.BlogCategory)
                .Include(b => b.User)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
