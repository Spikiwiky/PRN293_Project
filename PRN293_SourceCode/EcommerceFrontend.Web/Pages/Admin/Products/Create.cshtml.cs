//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using EcommerceFrontend.Web.Models;
//using EcommerceFrontend.Web.Services;
//using System.Threading.Tasks;

//namespace EcommerceFrontend.Web.Pages.Admin.Products
//{
//    public class CreateModel : PageModel
//    {
//        private readonly IProductService _productService;
//        public CreateModel(IProductService productService)
//        {
//            _productService = productService;
//        }
//        [BindProperty]
//        public ProductDTO Product { get; set; }
//        public async Task<IActionResult> OnPostAsync()
//        {
//            var success = await _productService.CreateProductAsync(Product);
//            if (success)
//                return RedirectToPage("Index");
//            ModelState.AddModelError(string.Empty, "Tạo sản phẩm thất bại.");
//            return Page();
//        }
//    }
//} 