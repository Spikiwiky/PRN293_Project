using EcommerceBackend.BusinessObject.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Abstract
{
    public interface ICommentService
    {
        Task<CommentDto> GetByIdAsync(int commentId);
        Task<IEnumerable<CommentDto>> GetByBlogIdAsync(int blogId);
        Task<IEnumerable<CommentDto>> GetByUserIdAsync(int userId);
        Task<CommentDto> CreateAsync(CreateCommentDto dto);
        Task<CommentDto> UpdateAsync(UpdateCommentDto dto);
        Task<bool> DeleteAsync(int commentId, bool hardDelete = false);
        Task<int> GetCommentCountForBlogAsync(int blogId);
    }
}