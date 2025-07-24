using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services.Admin.BlogCategory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace EcommerceFrontend.WebApp.Pages.Admin.BlogCategory
{
    public class CreateModel : PageModel
    {
        private readonly IBlogCategoryServiceWeb _service;

        public CreateModel(IBlogCategoryServiceWeb service)
        {
            _service = service;
        }

        [BindProperty]
        public CreateBlogCategoryDto BlogCategory { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _service.CreateCategoryAsync(BlogCategory);
            return RedirectToPage("./Index");
        }
    }
}
