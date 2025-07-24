using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Services.Admin.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommerceFrontend.Web.Pages.Admin.Blogs
{
    public class CreateModel : PageModel
    {
        private readonly IAdminBlogService _blogService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            IAdminBlogService blogService,
            ILogger<CreateModel> logger)
        {
            _blogService = blogService;
            _logger = logger;
            Blog = new AdminCreateBlogDto(); // Initialize here
        }

        [BindProperty]
        public AdminCreateBlogDto Blog { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                var categories = await _blogService.GetCategoriesAsync();
                Categories = categories?.Select(c => new SelectListItem
                {
                    Value = c.BlogCategoryId.ToString(),
                    Text = c.BlogCategoryTitle
                }).ToList() ?? new List<SelectListItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
                Categories = new List<SelectListItem>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            try
            {
                await _blogService.CreateBlogAsync(Blog);
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating blog");
                ModelState.AddModelError("", "Error creating blog. Please try again.");
                await LoadCategoriesAsync();
                return Page();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await _blogService.GetCategoriesAsync();
                Categories = categories?.Select(c => new SelectListItem
                {
                    Value = c.BlogCategoryId.ToString(),
                    Text = c.BlogCategoryTitle

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