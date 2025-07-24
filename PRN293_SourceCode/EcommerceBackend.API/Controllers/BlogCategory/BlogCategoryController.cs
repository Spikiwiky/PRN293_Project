using EcommerceBackend.BusinessObject.Abstract.BlogCategoryAbstract;
using EcommerceBackend.BusinessObject.dtos;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BlogCategoriesController : ControllerBase
{
    private readonly IBlogCategoryService _service;

    public BlogCategoriesController(IBlogCategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlogCategoryDto>>> GetAll(
        [FromQuery] bool includeDeleted = false)
    {
        var categories = await _service.GetAllCategoriesAsync(includeDeleted);
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BlogCategoryDto>> GetById(
        int id,
        [FromQuery] bool includeDeleted = false)
    {
        var category = await _service.GetCategoryByIdAsync(id, includeDeleted);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<BlogCategoryDto>> Create(
    [FromBody] CreateBlogCategoryDto dto)
    {
        var createdCategory = await _service.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(GetById),
            new { id = createdCategory.BlogCategoryId },
            createdCategory);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateBlogCategoryDto dto)
    {
        if (id != dto.BlogCategoryId) return BadRequest("ID mismatch");

        try
        {
            await _service.UpdateCategoryAsync(dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteCategoryAsync(id, hardDelete: false);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}/hard")]
    public async Task<IActionResult> HardDelete(int id)
    {
        try
        {
            await _service.DeleteCategoryAsync(id, hardDelete: true);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        try
        {
            await _service.RestoreCategoryAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}