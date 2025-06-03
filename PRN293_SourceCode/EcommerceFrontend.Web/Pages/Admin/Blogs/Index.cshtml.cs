using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Services.Admin.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages.Admin.Blogs
{
    public class IndexModel : PageModel
    {
        private readonly IAdminBlogService _blogService;

        public IndexModel(IAdminBlogService blogService)
        {
            _blogService = blogService;
        }

        public List<AdminBlogDto> Blogs { get; set; }

        public async Task OnGetAsync()
        {
            Blogs = await _blogService.GetBlogsAsync();
        }
    }
}
