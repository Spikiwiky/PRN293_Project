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
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        /// <summary>
        /// Get all blogs
        /// </summary>
        /// <param name="includeDeleted">Include deleted blogs (admin only)</param>
        /// <returns>List of blogs</returns>
        
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<BlogDto>>> GetAllBlogs(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool includeDeleted = false)
        {
            try
            {
                var blogs = await _blogService.GetAllBlogsAsync(includeDeleted);
                var paginated = new PaginatedResponse<BlogDto>
                {
                    Items = blogs.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                    TotalCount = blogs.Count(),
                    Page = page,
                    PageSize = pageSize
                };
                return Ok(paginated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get blog by ID
        /// </summary>
        /// <param name="id">Blog ID</param>
        /// <param name="includeDeleted">Include deleted blog (admin only)</param>
        /// <returns>Blog details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDetailDto>> GetBlogById(int id, bool includeDeleted = false)
        {
            try
            {
                var blog = await _blogService.GetBlogByIdAsync(id, includeDeleted);
                if (blog == null)
                {
                    return NotFound();
                }
                return Ok(blog);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create new blog
        /// </summary>
        /// <param name="dto">Blog creation data</param>
        /// <returns>Created blog</returns>
        [HttpPost]
        public async Task<ActionResult<BlogDto>> CreateBlog([FromBody] CreateBlogDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdBlog = await _blogService.CreateBlogAsync(dto);
                return CreatedAtAction(nameof(GetBlogById), new { id = createdBlog.BlogId }, createdBlog);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update existing blog
        /// </summary>
        /// <param name="id">Blog ID</param>
        /// <param name="dto">Blog update data</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(int id, [FromBody] UpdateBlogDto dto)
        {
            try
            {
                if (id != dto.BlogId)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _blogService.UpdateBlogAsync(dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete blog (soft delete by default)
        /// </summary>
        /// <param name="id">Blog ID</param>
        /// <param name="hardDelete">Permanently delete blog</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id, [FromQuery] bool hardDelete = false)
        {
            try
            {
                await _blogService.DeleteBlogAsync(id, hardDelete);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Restore deleted blog
        /// </summary>
        /// <param name="id">Blog ID</param>
        /// <returns></returns>
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestoreBlog(int id)
        {
            try
            {
                await _blogService.RestoreBlogAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get blogs by category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of blogs in category</returns>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetBlogsByCategory(int categoryId)
        {
            try
            {
                var blogs = await _blogService.GetBlogsByCategoryAsync(categoryId);
                return Ok(blogs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Search blogs by title
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="includeDeleted">Include deleted blogs (admin only)</param>
        /// <returns>Matching blogs</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BlogDto>>> SearchBlogs(
            [FromQuery] string searchTerm,
            [FromQuery] bool includeDeleted = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest("Search term is required");
                }

                var blogs = await _blogService.SearchBlogsAsync(searchTerm, includeDeleted);
                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}