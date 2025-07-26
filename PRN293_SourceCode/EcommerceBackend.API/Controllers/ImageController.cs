using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace EcommerceBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("products/{imageName}")]
        public IActionResult GetProductImage(string imageName)
        {
            try
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", imageName);
                
                if (!System.IO.File.Exists(imagePath))
                {
                    // Return default image if the requested image doesn't exist
                    var defaultImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "default-product.jpg");
                    if (System.IO.File.Exists(defaultImagePath))
                    {
                        var defaultImageBytes = System.IO.File.ReadAllBytes(defaultImagePath);
                        return File(defaultImageBytes, "image/jpeg");
                    }
                    
                    return NotFound();
                }

                var imageBytes = System.IO.File.ReadAllBytes(imagePath);
                var contentType = GetContentType(imageName);
                
                return File(imageBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("default")]
        public IActionResult GetDefaultImage()
        {
            try
            {
                var defaultImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "default-product.jpg");
                
                if (!System.IO.File.Exists(defaultImagePath))
                {
                    return NotFound();
                }

                var imageBytes = System.IO.File.ReadAllBytes(defaultImagePath);
                return File(imageBytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
} 