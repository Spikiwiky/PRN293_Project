using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services.Admin.BlogCategory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace EcommerceFrontend.WebApp.Pages.Admin.BlogCategory
{
    public class EditModel : PageModel
    {
        private readonly IBlogCategoryServiceWeb _service;

        public EditModel(IBlogCategoryServiceWeb service)
        {
            _service = service;
        }

        [BindProperty]
        public UpdateBlogCategoryDto BlogCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var data = await _service.GetCategoryByIdAsync(id);

                if (data == null)
                    return NotFound();

                BlogCategory = new UpdateBlogCategoryDto
                {
                    BlogCategoryId = data.BlogCategoryId,
                    BlogCategoryTitle = data.BlogCategoryTitle,
                    IsDelete = data.IsDelete
                };

                return Page();
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _service.UpdateCategoryAsync(BlogCategory);
                return RedirectToPage("./Index");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Update failed");
                return Page();
            }
        }
    }
}
