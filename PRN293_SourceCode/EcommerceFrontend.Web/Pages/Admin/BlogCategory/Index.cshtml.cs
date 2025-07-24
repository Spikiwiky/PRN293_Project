
using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services.Admin.BlogCategory;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages.Admin.BlogCategory
{
    public class IndexModel : PageModel
    {
        private readonly IBlogCategoryServiceWeb _categoryService;

        public IndexModel(IBlogCategoryServiceWeb categoryService)
        {
            _categoryService = categoryService;
        }

        public List<BlogCategoryDto> Categories { get; set; } = new();

        public async Task OnGetAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync(includeDeleted: true);
            Categories = categories.ToList();
        }
    }
}