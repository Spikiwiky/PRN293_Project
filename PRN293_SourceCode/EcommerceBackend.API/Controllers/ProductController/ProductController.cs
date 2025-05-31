
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EcommerceBackend.BusinessObject.Services;
using EcommerceBackend.BusinessObject.Services.ProductService;
namespace EcommerceBackend.API.Controllers.ProductController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("load")]
        public async Task<IActionResult> LoadProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productService.LoadProductsAsync(page, pageSize);
            return Ok(products);
        }
       
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string name = null,
            [FromQuery] string category = null,
            [FromQuery] string size = null,
            [FromQuery] string color = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var products = await _productService.SearchProductsAsync(name, category, size, color, page, pageSize);
            return Ok(products);
        }
    }
}
