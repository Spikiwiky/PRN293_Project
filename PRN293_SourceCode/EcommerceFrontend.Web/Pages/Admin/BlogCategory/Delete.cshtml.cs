using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services.Admin.BlogCategory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages.Admin.BlogCategory
{
    public class DeleteModel : PageModel
    {
        private readonly IBlogCategoryServiceWeb _categoryService;

        public DeleteModel(IBlogCategoryServiceWeb categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
        public BlogCategoryDto? Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Category = await _categoryService.GetCategoryByIdAsync(id);
            if (Category == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
          

              await _categoryService.DeleteCategoryAsync(Category.BlogCategoryId);

        

            return RedirectToPage("./Index");
        }
    }
}
