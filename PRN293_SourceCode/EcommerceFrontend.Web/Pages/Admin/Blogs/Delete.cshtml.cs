using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Services.Admin.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace EcommerceFrontend.Web.Pages.Admin.Blogs
{
    public class DeleteModel : PageModel
    {
        private readonly IAdminBlogService _blogService;

        public DeleteModel(IAdminBlogService blogService)
        {
            _blogService = blogService;
        }

        [BindProperty]
        public AdminBlogDto Blog { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Blog = await _blogService.GetBlogByIdAsync(id);

            if (Blog == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Blog == null || Blog.BlogId <= 0)
            {
                return BadRequest();
            }

            await _blogService.DeleteBlogAsync(Blog.BlogId, false);
            return RedirectToPage("./Index");
        }
    }
}
