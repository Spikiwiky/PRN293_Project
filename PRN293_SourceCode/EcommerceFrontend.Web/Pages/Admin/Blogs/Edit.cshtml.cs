using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Services.Admin.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommerceFrontend.Web.Pages.Admin.Blogs
{
    public class EditModel : PageModel
    {
        private readonly IAdminBlogService _blogService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            IAdminBlogService blogService,
            ILogger<EditModel> logger)
        {
            _blogService = blogService;
            _logger = logger;
        }

        [BindProperty]
        public AdminUpdateBlogDto Blog { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var blogDetail = await _blogService.GetBlogByIdAsync(id);
                if (blogDetail == null || blogDetail.IsDelete)
                {
                    return NotFound();
                }

                Blog = new AdminUpdateBlogDto
                {
                    BlogId = blogDetail.BlogId,
                    Title = blogDetail.Title,
                    Content = blogDetail.Content, // ✅ Use Content instead of ContentPreview
                    BlogCategoryId = blogDetail.Category?.BlogCategoryId
                };

                await LoadCategoriesAsync(blogDetail.Category?.BlogCategoryId);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading blog detail");
                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(Blog.BlogCategoryId);
                return Page();
            }

            try
            {
                await _blogService.UpdateBlogAsync(Blog);
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blog");
                ModelState.AddModelError("", "Lỗi khi cập nhật blog. Vui lòng thử lại.");
                await LoadCategoriesAsync(Blog.BlogCategoryId);
                return Page();
            }
        }

        private async Task LoadCategoriesAsync(int? selectedCategoryId)
        {
            try
            {
                var categories = await _blogService.GetCategoriesAsync();
                Categories = categories?
                    .Where(c => !c.IsDelete)
                    .Select(c => new SelectListItem
                    {
                        Value = c.BlogCategoryId.ToString(),
                        Text = c.BlogCategoryTitle,
                        Selected = c.BlogCategoryId == selectedCategoryId
                    }).ToList() ?? new List<SelectListItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
                Categories = new List<SelectListItem>();
            }
        }
    }
}
