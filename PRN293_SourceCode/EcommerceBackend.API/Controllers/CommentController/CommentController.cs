using EcommerceBackend.BusinessObject.Abstract;
using EcommerceBackend.BusinessObject.Abstract.BlogAbstract;
using EcommerceBackend.BusinessObject.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IBlogService _blogService;

        public CommentController(
            ICommentService commentService,
            IBlogService blogService)
        {
            _commentService = commentService;
            _blogService = blogService;
        }

        /// <summary>
        /// Get comment by ID
        /// </summary>
        /// <param name="commentId">Comment ID</param>
        /// <returns>Comment details</returns>
        [HttpGet("{commentId}")]
        public async Task<ActionResult<CommentDto>> GetById(int commentId)
        {
            try
            {
                var comment = await _commentService.GetByIdAsync(commentId);
                if (comment == null)
                {
                    return NotFound();
                }
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all comments for a specific blog
        /// </summary>
        /// <param name="blogId">Blog ID</param>
        /// <returns>List of comments</returns>
        [HttpGet("blog/{blogId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetByBlogId(int blogId)
        {
            try
            {
                // Verify blog exists
                var blog = await _blogService.GetBlogByIdAsync(blogId);
                if (blog == null)
                {
                    return NotFound("Blog not found");
                }

                var comments = await _commentService.GetByBlogIdAsync(blogId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all comments by a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of comments</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetByUserId(int userId)
        {
            try
            {
                var comments = await _commentService.GetByUserIdAsync(userId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get comment count for a blog
        /// </summary>
        /// <param name="blogId">Blog ID</param>
        /// <returns>Comment count</returns>
        [HttpGet("blog/{blogId}/count")]
        public async Task<ActionResult<int>> GetCommentCountForBlog(int blogId)
        {
            try
            {
                // Verify blog exists
                var blog = await _blogService.GetBlogByIdAsync(blogId);
                if (blog == null)
                {
                    return NotFound("Blog not found");
                }

                var count = await _commentService.GetCommentCountForBlogAsync(blogId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create new comment
        /// </summary>
        /// <param name="dto">Comment creation data</param>
        /// <returns>Created comment</returns>
        [HttpPost]
        public async Task<ActionResult<CommentDto>> Create([FromBody] CreateCommentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verify blog exists
                var blog = await _blogService.GetBlogByIdAsync(dto.BlogId);
                if (blog == null)
                {
                    return NotFound("Blog not found");
                }

                var createdComment = await _commentService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { commentId = createdComment.CommentId }, createdComment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update existing comment
        /// </summary>
        /// <param name="dto">Comment update data</param>
        /// <returns>Updated comment</returns>
        [HttpPut]
        public async Task<ActionResult<CommentDto>> Update([FromBody] UpdateCommentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedComment = await _commentService.UpdateAsync(dto);
                if (updatedComment == null)
                {
                    return NotFound();
                }
                return Ok(updatedComment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete comment
        /// </summary>
        /// <param name="commentId">Comment ID</param>
        /// <param name="hardDelete">Permanently delete comment</param>
        /// <returns>Success status</returns>
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> Delete(int commentId, [FromQuery] bool hardDelete = false)
        {
            try
            {
                var result = await _commentService.DeleteAsync(commentId, hardDelete);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}