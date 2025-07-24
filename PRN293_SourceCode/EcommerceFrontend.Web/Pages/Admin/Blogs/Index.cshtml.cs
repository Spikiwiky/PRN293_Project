using EcommerceFrontend.Web.Models.Admin;
using EcommerceFrontend.Web.Services.Admin.Blog;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace EcommerceFrontend.Web.Pages.Admin.Blogs
{
    public class IndexModel : PageModel
    {
        private readonly IAdminBlogService _blogService;

        public IndexModel(IAdminBlogService blogService)
        {
            _blogService = blogService;
        }

        public PaginatedResponse<AdminBlogDto> BlogsResponse { get; set; }

        public async Task OnGetAsync(int page = 1, int pageSize = 10)
        {
            BlogsResponse = await _blogService.GetBlogsAsync(page, pageSize);

            // Add debug output
            System.Diagnostics.Debug.WriteLine($"Fetched {BlogsResponse?.Items?.Count ?? 0} blogs");
        }
    }
}