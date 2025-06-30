//using EcommerceBackend.DataAccess.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace EcommerceBackend.API.Controllers.SaleController
//{
//    [Route("api/sale/categories")]
//    [ApiController]
//    public class SaleCategoryController : ControllerBase
//    {
//        private readonly EcommerceDBContext _context;
//        private readonly ILogger<SaleCategoryController> _logger;

//        public SaleCategoryController(
//            EcommerceDBContext context,
//            ILogger<SaleCategoryController> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetCategories()
//        {
//            try
//            {
//                var categories = await _context.ProductCategories
//                    .Where(c => c.IsDelete != true)
//                    .Select(c => new { Id = c.ProductCategoryId, Name = c.ProductCategoryTitle })
//                    .ToListAsync();

//                return Ok(categories);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting categories");
//                return StatusCode(500, "An error occurred while retrieving categories");
//            }
//        }
//    }
//}
