using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages.Blog
{
    public class IndexModel : PageModel
    {
        private readonly BlogService _blogService;
        public List<BlogDto> Blogs { get; set; } = new();

        public IndexModel(BlogService blogService)
        {
            _blogService = blogService;
        }

        public async Task OnGetAsync()
        {
            Blogs = await _blogService.GetBlogsAsync();
        }
    }
}
