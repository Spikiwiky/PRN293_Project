using EcommerceBackend.BusinessObject.Services.SaleService.BlogService;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using EcommerceBackend.API.Dtos.Sale;
using EcommerceBackend.DataAccess.Models;

namespace EcommerceBackend.API.Controllers.SaleController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleBlogController : ControllerBase
    {
        private readonly ISaleBlogService _service;

        public SaleBlogController(ISaleBlogService service)
        {
            _service = service;
        }

        // GET: api/saleblog
        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _service.GetAllBlogsAsync();

            var result = blogs.Select(b => new BlogDto
            {
                BlogId = b.BlogId,
                BlogTittle = b.BlogTittle,
                BlogSummary = b.BlogSummary,
                BlogImage = b.BlogImage,
                BlogCategoryTitle = b.BlogCategory?.BlogCategoryTitle,
                PublishedDate = b.PublishedDate,
                IsPublished = b.IsPublished,
                ViewCount = b.ViewCount
            }).ToList();

            return Ok(result);
        }

        // GET: api/saleblog/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var blog = await _service.GetBlogByIdAsync(id);
            if (blog == null) return NotFound();

            var result = new BlogDetailDto
            {
                BlogId = blog.BlogId,
                BlogTittle = blog.BlogTittle,
                BlogContent = blog.BlogContent,
                BlogSummary = blog.BlogSummary,
                BlogImage = blog.BlogImage,
                BlogCategoryTitle = blog.BlogCategory?.BlogCategoryTitle,
                PublishedDate = blog.PublishedDate,
                IsPublished = blog.IsPublished,
                ViewCount = blog.ViewCount,
                Comments = blog.Comments.Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    CommenterName = c.CommenterName,
                    CommentContent = c.CommentContent,
                    CreatedDate = c.CreatedDate,
                    IsApproved = c.IsApproved
                }).ToList()
            };

            return Ok(result);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateBlog([FromForm] BlogCreateFormDto dto, [FromForm] string? imageUrl)
        {
            var blog = new Blog
            {
                BlogCategoryId = dto.BlogCategoryId,
                BlogTittle = dto.BlogTittle,
                BlogContent = dto.BlogContent,
                BlogSummary = dto.BlogSummary,
                IsPublished = dto.IsPublished,
                PublishedDate = dto.PublishedDate ?? DateTime.UtcNow
            };

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "blogs");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }
                blog.BlogImage = $"/images/blogs/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                blog.BlogImage = imageUrl;
            }

            await _service.CreateBlogAsync(blog);

            return CreatedAtAction(nameof(GetBlogById), new { id = blog.BlogId }, blog);
        }


        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateBlog(int id, [FromForm] BlogUpdateFormDto dto)
        {
            var existing = await _service.GetBlogByIdAsync(id);
            if (existing == null) return NotFound();

            if (dto.RemoveImage && dto.ImageFile == null)
            {
                if (!string.IsNullOrEmpty(existing.BlogImage))
                {
                    if (existing.BlogImage.StartsWith("/images/blogs/"))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.BlogImage.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                }
                existing.BlogImage = null;
            }
            else if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "blogs");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }
                existing.BlogImage = $"/images/blogs/{fileName}";
            }
            existing.BlogCategoryId = dto.BlogCategoryId;
            existing.BlogTittle = dto.BlogTittle;
            existing.Tags = dto.Tags;
            existing.BlogContent = dto.BlogContent;
            existing.BlogSummary = dto.BlogSummary;
            existing.IsPublished = dto.IsPublished;

            await _service.UpdateBlogAsync(existing);
            return NoContent();
        }


        // DELETE: api/saleblog/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            await _service.DeleteBlogAsync(id);
            return NoContent();
        }

        // GET: api/saleblog/categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _service.GetAllCategoriesAsync();
            var result = categories.Select(c => new BlogCategoryDto
            {
                BlogCategoryId = c.BlogCategoryId,
                BlogCategoryTitle = c.BlogCategoryTitle
            }).ToList();
            return Ok(result);
        }

        // GET: api/saleblog/{id}/comments
        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments(int id)
        {
            var comments = await _service.GetCommentsByBlogIdAsync(id);
            var result = comments.Select(c => new CommentDto
            {
                CommentId = c.CommentId,
                CommenterName = c.CommenterName,
                CommentContent = c.CommentContent,
                CreatedDate = c.CreatedDate,
                IsApproved = c.IsApproved
            }).ToList();
            return Ok(result);
        }

        // POST: api/saleblog/{id}/comments
        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(int id, [FromBody] CommentDto dto)
        {
            var comment = await _service.AddCommentAsync(new DataAccess.Models.BlogComment
            {
                BlogId = id,
                CommenterName = dto.CommenterName,
                CommentContent = dto.CommentContent,
                CreatedDate = DateTime.UtcNow,
                IsApproved = dto.IsApproved
            });

            return CreatedAtAction(nameof(GetComments), new { id = id }, new CommentDto
            {
                CommentId = comment.CommentId,
                CommenterName = comment.CommenterName,
                CommentContent = comment.CommentContent,
                CreatedDate = comment.CreatedDate,
                IsApproved = comment.IsApproved
            });
        }

        // GET: api/saleblog/paged?search=abc&pageNumber=1&pageSize=10&categoryId=1
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedBlogs([FromQuery] string? search, [FromQuery] int? categoryId,
                                                       [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var blogs = await _service.GetAllBlogsAsync();

            if (!string.IsNullOrWhiteSpace(search))
                blogs = blogs.Where(b => (b.BlogTittle ?? "").Contains(search, StringComparison.OrdinalIgnoreCase));

            if (categoryId.HasValue)
                blogs = blogs.Where(b => b.BlogCategoryId == categoryId);

            var total = blogs.Count();
            var items = blogs.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(b => new BlogDto
            {
                BlogId = b.BlogId,
                BlogTittle = b.BlogTittle,
                BlogSummary = b.BlogSummary,
                BlogImage = b.BlogImage,
                BlogCategoryTitle = b.BlogCategory?.BlogCategoryTitle,
                PublishedDate = b.PublishedDate,
                IsPublished = b.IsPublished,
                ViewCount = b.ViewCount
            }).ToList();

            var result = new PagedResultDto<BlogDto>
            {
                Items = items,
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        // GET: api/saleblog/tags?tag=keyword
        [HttpGet("tags")]
        public async Task<IActionResult> GetByTag([FromQuery] string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return BadRequest("Tag không hợp lệ.");

            var blogs = await _service.GetAllBlogsAsync();
            var filtered = blogs
                .Where(b => !string.IsNullOrEmpty(b.Tags) && b.Tags.Contains(tag, StringComparison.OrdinalIgnoreCase))
                .Select(b => new BlogDto
                {
                    BlogId = b.BlogId,
                    BlogTittle = b.BlogTittle,
                    BlogSummary = b.BlogSummary,
                    BlogImage = b.BlogImage,
                    BlogCategoryTitle = b.BlogCategory?.BlogCategoryTitle,
                    PublishedDate = b.PublishedDate,
                    IsPublished = b.IsPublished,
                    ViewCount = b.ViewCount
                }).ToList();

            return Ok(filtered);
        }

        // GET: api/saleblog/view/{id}
        [HttpGet("view/{id}")]
        public async Task<IActionResult> ViewBlog(int id)
        {
            var blog = await _service.GetBlogByIdAsync(id);
            if (blog == null) return NotFound();

            blog.ViewCount += 1;
            await _service.UpdateBlogAsync(blog);

            return Ok(new { message = "View count updated", viewCount = blog.ViewCount });
        }

        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetCategory(int id) => Ok(await _service.GetCategoryByIdAsync(id));

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            if (dto == null) return BadRequest();

            var category = new BlogCategory
            {
                BlogCategoryTitle = dto.BlogCategoryTitle,
                IsDelete = false
            };

            var created = await _service.CreateCategoryAsync(category);

            var result = new BlogCategoryDto
            {
                BlogCategoryTitle = created.BlogCategoryTitle
            };

            return CreatedAtAction(nameof(GetCategory), new { id = created.BlogCategoryId }, result);
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] BlogCategoryDto dto)
        {
            if (dto == null) return BadRequest();

            var category = new BlogCategory
            {
                BlogCategoryId = id,
                BlogCategoryTitle = dto.BlogCategoryTitle,
                IsDelete = dto.IsDelete
            };

            var updated = await _service.UpdateCategoryAsync(category);
            if (updated == null) return NotFound();

            return Ok(new BlogCategoryDto
            {
                BlogCategoryId = updated.BlogCategoryId,
                BlogCategoryTitle = updated.BlogCategoryTitle,
                IsDelete = updated.IsDelete ?? false
            });
        }


        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id) => Ok(await _service.DeleteCategoryAsync(id));

    }
}
