using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly EcommerceDBContext _context;

        public CommentRepository(EcommerceDBContext context)
        {
            _context = context;
        }

        public async Task<Comment> GetByIdAsync(int commentId)
        {
            return await _context.Comments
                .Include(c => c.Blog)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CommentId == commentId);
        }

        // Original method (without includeDeleted parameter)
        public async Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId)
        {
            return await GetByBlogIdAsync(blogId, includeDeleted: false);
        }

        // New overload with includeDeleted parameter
        public async Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId, bool includeDeleted)
        {
            IQueryable<Comment> query = _context.Comments  // Explicitly declare as IQueryable<Comment>
                .Include(c => c.User)
                .Where(c => c.BlogId == blogId);

            if (!includeDeleted)
            {
                query = query.Where(c => !c.IsDeleted);
            }

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetByUserIdAsync(int userId)
        {
            return await _context.Comments
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .Include(c => c.Blog)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            comment.CreatedAt = DateTime.UtcNow;
            comment.IsDeleted = false;

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(comment.CommentId);
        }

        public async Task<Comment> UpdateAsync(Comment comment)
        {
            var existingComment = await _context.Comments.FindAsync(comment.CommentId);
            if (existingComment == null)
                return null;

            existingComment.Content = comment.Content;
            existingComment.IsDeleted = comment.IsDeleted;

            _context.Comments.Update(existingComment);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(existingComment.CommentId);
        }

        public async Task<bool> SoftDeleteAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || comment.IsDeleted)
                return false;

            comment.IsDeleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> HardDeleteAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetCommentCountForBlogAsync(int blogId)
        {
            return await _context.Comments
                .CountAsync(c => c.BlogId == blogId && !c.IsDeleted);
        }
    }
}