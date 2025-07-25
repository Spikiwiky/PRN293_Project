using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            // Redirect to Homepage
            Response.Redirect("/CommonPage/Homepage");
        }
    }
} 