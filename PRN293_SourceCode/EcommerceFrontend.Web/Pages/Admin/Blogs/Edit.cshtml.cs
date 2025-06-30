using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services.Admin.Blog;
using EcommerceFrontend.Web.Services.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages.Admin.Blogs
{
    public class EditModel : PageModel
    {
        private readonly IBlogService _blogService;

        public EditModel(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [BindProperty]
        public BlogDto Blog { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Blog = await _blogService.GetBlogByIdAsync(id);
            if (Blog == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            await _blogService.UpdateBlogAsync(Blog);
            return RedirectToPage("./Index");
        }
    }
}
