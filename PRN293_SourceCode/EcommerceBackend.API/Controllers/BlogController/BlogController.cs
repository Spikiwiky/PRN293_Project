using Microsoft.AspNetCore.Mvc;

namespace EcommerceBackend.API.Controllers.BlogController
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
