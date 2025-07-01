using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly IBlogService _blogService;

        public DetailsModel(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public BlogDto Blog { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Blog = await _blogService.GetBlogByIdAsync(id);
            if (Blog == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}