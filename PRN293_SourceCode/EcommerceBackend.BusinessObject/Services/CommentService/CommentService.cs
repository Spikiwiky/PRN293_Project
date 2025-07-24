using EcommerceBackend.BusinessObject.Abstract;
using EcommerceBackend.BusinessObject.Dtos;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Abstract.BlogAbstract;
using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.DataAccess.Repository.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly IUserRepository _userRepository;

        public CommentService(
            ICommentRepository commentRepository,
            IBlogRepository blogRepository,
            IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _blogRepository = blogRepository;
            _userRepository = userRepository;
        }

        public async Task<CommentDto> GetByIdAsync(int commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            return comment == null ? null : MapToDto(comment);
        }

        public async Task<IEnumerable<CommentDto>> GetByBlogIdAsync(int blogId)
        {
            var comments = await _commentRepository.GetByBlogIdAsync(blogId);
            return comments.Select(MapToDto);
        }

        public async Task<IEnumerable<CommentDto>> GetByUserIdAsync(int userId)
        {
            var comments = await _commentRepository.GetByUserIdAsync(userId);
            return comments.Select(MapToDto);
        }

        public async Task<CommentDto> CreateAsync(CreateCommentDto dto)
        {
            // Lazy validation - get full blog if needed
            var blog = await _blogRepository.GetByIdAsync(dto.BlogId);
            if (blog == null) throw new ArgumentException("Blog not found");

            // Only validate user if UserId is provided
            if (dto.UserId.HasValue && _userRepository.GetById(dto.UserId.Value) == null)
            {
                throw new ArgumentException("User not found");
            }

            var comment = new Comment
            {
                BlogId = dto.BlogId,
                UserId = (int)dto.UserId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            var createdComment = await _commentRepository.CreateAsync(comment);
            return MapToDto(createdComment);
        }

        public async Task<CommentDto> UpdateAsync(UpdateCommentDto dto)
        {
            var comment = await _commentRepository.GetByIdAsync(dto.CommentId);
            if (comment == null)
                throw new KeyNotFoundException("Comment not found");

            comment.Content = dto.Content;
            var updatedComment = await _commentRepository.UpdateAsync(comment);
            return MapToDto(updatedComment);
        }

        public async Task<bool> DeleteAsync(int commentId, bool hardDelete = false)
        {
            return hardDelete
                ? await _commentRepository.HardDeleteAsync(commentId)
                : await _commentRepository.SoftDeleteAsync(commentId);
        }

        public async Task<int> GetCommentCountForBlogAsync(int blogId)
        {
            return await _commentRepository.GetCommentCountForBlogAsync(blogId);
        }

        private CommentDto MapToDto(Comment comment)
        {
            return new CommentDto
            {
                CommentId = comment.CommentId,
                BlogId = comment.BlogId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                Author = comment.User?.UserName ?? "Anonymous"
                
            };
        }
    }
}