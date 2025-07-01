using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services.Admin.Blog;
using EcommerceFrontend.Web.Services.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages.Admin.Blogs
{
    public class CreateModel : PageModel
    {
        private readonly IBlogService _blogService;

        public CreateModel(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [BindProperty]
        public BlogDto Blog { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _blogService.CreateBlogAsync(Blog);
            return RedirectToPage("Index");
        }
    }
}
