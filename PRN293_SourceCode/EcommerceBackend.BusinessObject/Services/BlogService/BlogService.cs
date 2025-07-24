using EcommerceBackend.BusinessObject.Abstract.BlogAbstract;
using EcommerceBackend.BusinessObject.dtos;
using EcommerceBackend.BusinessObject.Dtos;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Abstract.BlogAbstract;
using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogCategoryRepository _categoryRepository;
        private readonly ICommentRepository _commentRepository;

        public BlogService(
            IBlogRepository blogRepository,
            IBlogCategoryRepository categoryRepository,
            ICommentRepository commentRepository)
        {
            _blogRepository = blogRepository;
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;
        }

        public async Task<IEnumerable<BlogDto>> GetAllBlogsAsync(bool includeDeleted = false)
        {
            var blogs = await _blogRepository.GetAllAsync(includeDeleted);
            return blogs.Select(MapToDto).ToList();
        }

        public async Task<BlogDetailDto> GetBlogByIdAsync(int id, bool includeDeleted = false)
        {
            var blog = await _blogRepository.GetByIdAsync(id, includeDeleted);
            if (blog == null) return null;

            var comments = includeDeleted
                ? await _commentRepository.GetByBlogIdAsync(blog.BlogId, includeDeleted: true)
                : await _commentRepository.GetByBlogIdAsync(blog.BlogId);

            return new BlogDetailDto
            {
                BlogId = blog.BlogId,
                Title = blog.BlogTittle,
                Content = blog.BlogContent,
                CreatedAt = blog.CreatedAt,
                IsDelete = blog.IsDelete,
                Category = blog.BlogCategory != null ? new BlogCategoryDto
                {
                    BlogCategoryId = blog.BlogCategory.BlogCategoryId,
                    BlogCategoryTitle = blog.BlogCategory.BlogCategoryTitle
                } : null,
                Comments = comments.Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    Author = c.User != null ? c.User.UserName : "Anonymous",
                    
                }).ToList()
            };
        }

        public async Task<BlogDto> CreateBlogAsync(CreateBlogDto dto)
        {
            if (dto.BlogCategoryId.HasValue && !await _categoryRepository.ExistsAsync(dto.BlogCategoryId.Value))
            {
                throw new ArgumentException("Invalid blog category ID");
            }

            var blog = new Blog
            {
                BlogTittle = dto.Title,
                BlogContent = dto.Content,
                BlogCategoryId = dto.BlogCategoryId,
                CreatedAt = DateTime.UtcNow,
                IsDelete = false
            };

            await _blogRepository.AddAsync(blog);
            return MapToDto(blog);
        }

        public async Task UpdateBlogAsync(UpdateBlogDto dto)
        {
            var blog = await _blogRepository.GetByIdAsync(dto.BlogId, includeDeleted: true);
            if (blog == null) throw new KeyNotFoundException("Blog not found");

            if (blog.IsDelete) throw new InvalidOperationException("Cannot update a deleted blog");

            if (dto.BlogCategoryId.HasValue && dto.BlogCategoryId != blog.BlogCategoryId)
            {
                if (!await _categoryRepository.ExistsAsync(dto.BlogCategoryId.Value))
                {
                    throw new ArgumentException("Invalid blog category ID");
                }
            }

            blog.BlogTittle = dto.Title ?? blog.BlogTittle;
            blog.BlogContent = dto.Content ?? blog.BlogContent;
            blog.BlogCategoryId = dto.BlogCategoryId ?? blog.BlogCategoryId;

            await _blogRepository.UpdateAsync(blog);
        }

        public async Task DeleteBlogAsync(int id, bool hardDelete = false)
        {
            if (hardDelete)
            {
                // First delete all comments
                var comments = await _commentRepository.GetByBlogIdAsync(id, includeDeleted: true);
                foreach (var comment in comments)
                {
                    await _commentRepository.HardDeleteAsync(comment.CommentId);
                }

                // Then delete the blog
                await _blogRepository.HardDeleteAsync(id);
            }
            else
            {
                await _blogRepository.SoftDeleteAsync(id);
            }
        }

        public async Task RestoreBlogAsync(int id)
        {
            await _blogRepository.RestoreAsync(id);
        }

        public async Task<IEnumerable<BlogDto>> GetBlogsByCategoryAsync(int categoryId)
        {
            if (!await _categoryRepository.ExistsAsync(categoryId))
            {
                throw new ArgumentException("Invalid category ID");
            }

            var blogs = await _blogRepository.GetByCategoryIdAsync(categoryId);
            return blogs.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<BlogDto>> SearchBlogsAsync(string searchTerm, bool includeDeleted = false)
        {
            var blogs = await _blogRepository.FindByTitleAsync(searchTerm, includeDeleted);
            return blogs.Select(MapToDto).ToList();
        }

        private BlogDto MapToDto(Blog blog)
        {
            return new BlogDto
            {
                BlogId = blog.BlogId,
                Title = blog.BlogTittle,
                ContentPreview = blog.BlogContent.Length > 100
                    ? blog.BlogContent.Substring(0, 100) + "..."
                    : blog.BlogContent,
                CreatedAt = blog.CreatedAt,
                IsDelete = blog.IsDelete,
                Category = blog.BlogCategory != null ? new BlogCategoryDto
                {
                    BlogCategoryId = blog.BlogCategory.BlogCategoryId, 
                    BlogCategoryTitle = blog.BlogCategory.BlogCategoryTitle
                } : null
            };
        }
    }
}