using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services.Admin.Blog;
using EcommerceFrontend.Web.Services.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages.Admin.Blogs
{
    public class IndexModel : PageModel
    {
        private readonly IBlogService _blogService;

        public IndexModel(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public List<BlogDto> Blogs { get; set; } = new();

        public async Task OnGetAsync()
        {
            Blogs = await _blogService.GetBlogsAsync();
        }
    }
}
