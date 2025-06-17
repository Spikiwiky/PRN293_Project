using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly BlogService _blogService;
        public BlogDto? Blog { get; set; }

        public DetailsModel(BlogService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Blog = await _blogService.GetBlogDetailsAsync(id);
            if (Blog == null) return NotFound();
            return Page();
        }
    }
}
