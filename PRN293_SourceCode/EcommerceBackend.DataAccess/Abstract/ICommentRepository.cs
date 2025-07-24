using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Abstract
{
    public interface ICommentRepository
    {
        Task<Comment> GetByIdAsync(int commentId);
        Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId, bool includeDeleted);
        Task<IEnumerable<Comment>> GetByUserIdAsync(int userId);
        Task<Comment> CreateAsync(Comment comment);
        Task<Comment> UpdateAsync(Comment comment);
        Task<bool> SoftDeleteAsync(int commentId);
        Task<bool> HardDeleteAsync(int commentId);
        Task<int> GetCommentCountForBlogAsync(int blogId);
        Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId);
        
    }
}
