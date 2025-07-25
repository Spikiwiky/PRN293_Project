using EcommerceBackend.BusinessObject.Services.SaleService;
using EcommerceFrontend.Web.Models;
using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Models.Sale;
using EcommerceFrontend.Web.Services;
using EcommerceFrontend.Web.Services.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace EcommerceFrontend.Web.Pages.CommonPage;

public class HomepageModel : PageModel
{
    private readonly IProductService _productService;
    private readonly IBlogService _blogService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiSettings _apiSettings;
    private readonly ILogger<HomepageModel> _logger;

    public HomepageModel(
        IProductService productService, 
        IBlogService blogService,
        IHttpClientFactory httpClientFactory, 
        IOptions<ApiSettings> apiSettings, 
        ILogger<HomepageModel> logger)
    {
        _productService = productService;
        _blogService = blogService;
        _httpClientFactory = httpClientFactory;
        _apiSettings = apiSettings.Value ?? throw new ArgumentNullException(nameof(apiSettings), "ApiSettings is not configured.");
        _logger = logger;
    }

    public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
    public List<BlogDto> Blogs { get; set; } = new List<BlogDto>();
    public ProductSearchParams SearchParams { get; set; } = new ProductSearchParams();
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public string? ErrorMessage { get; set; }
    public List<CategoryModel> Categories { get; set; } = new();
    public int PageIndex { get; set; } = 1;
    public string ApiBaseUrl => _apiSettings.BaseUrl;

    // Hardcode sản phẩm Featured
    public List<ProductDTO> FeaturedProducts { get; set; } = new List<ProductDTO>();
    
    // Hardcode sản phẩm Top Rate
    public List<ProductDTO> TopRateProducts { get; set; } = new List<ProductDTO>();
    
    // Hardcode sản phẩm Sale
    public List<ProductDTO> SaleProducts { get; set; } = new List<ProductDTO>();

    public async Task<IActionResult> OnGetAsync(
        string? name = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int page = 1)
    {
        try
        {
            SearchParams = new ProductSearchParams
            {
                Name = name,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Page = page,
                PageSize = PageSize
            };

            Products = await _productService.SearchProductsAsync(SearchParams);
            CurrentPage = page;
            PageIndex = page;

            // Get total count of products for pagination
            var totalProducts = await _productService.GetTotalProductsCountAsync(
                name: name,
                category: category,
                minPrice: minPrice,
                maxPrice: maxPrice
            );

            TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

            // Tạo sản phẩm Featured và Top Rate hardcode
            CreateFeaturedProducts();
            CreateTopRateProducts();
            CreateSaleProducts();

            // Load published blogs for homepage
            try
            {
                var blogClient = _httpClientFactory.CreateClient("MyAPI");
                var blogResponse = await blogClient.GetAsync($"{_apiSettings.BaseUrl}/api/blog/published");
                if (blogResponse.IsSuccessStatusCode)
                {
                    var allBlogs = await blogResponse.Content.ReadFromJsonAsync<List<BlogDto>>() ?? new List<BlogDto>();
                  
                    Blogs = allBlogs
                        .GroupBy(b => b.BlogId) // Group by BlogId to remove duplicates
                        .Select(g => g.First()) // Take the first occurrence of each blog
                        .OrderByDescending(b => b.PublishedDate ?? b.CreatedDate ?? DateTime.MinValue)
                        .Take(3)
                        .ToList();
                }
                else
                {
                    Blogs = new List<BlogDto>();
                }
            }
            catch (Exception blogEx)
            {
                _logger.LogWarning(blogEx, "Failed to load blogs for homepage");
                Blogs = new List<BlogDto>();
            }

            var categoryClient = _httpClientFactory.CreateClient("MyAPI");
            var fullUrl = $"{_apiSettings.BaseUrl}/api/sale/categories";
            var response = await categoryClient.GetAsync(fullUrl);
            if (response.IsSuccessStatusCode)
            {
                Categories = await response.Content.ReadFromJsonAsync<List<CategoryModel>>() ?? new List<CategoryModel>();
            }
            else
            {
                Categories = new List<CategoryModel>();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products");
            ErrorMessage = "An error occurred while loading products. Please try again later.";
            return Page();
        }
    }

    // Method tạo sản phẩm Featured hardcode
    private void CreateFeaturedProducts()
    {
        FeaturedProducts = new List<ProductDTO>
        {
            new ProductDTO
            {
                ProductId = 101,
                Name = "Áo Thun Nam Premium",
                Description = "Áo thun nam chất liệu cotton cao cấp, thiết kế đơn giản nhưng thanh lịch",
                Brand = "HIDDLE™",
                BasePrice = 200.00m,
                Images = new List<string> { "/images/products/aoThunNam.jpg" },
                CategoryName = "Men Clothing"
            },
            new ProductDTO
            {
                ProductId = 102,
                Name = "Mũ Mùa Hè",
                Description = "Mũ phớt mùa hè chất liệu cói tự nhiên, phong cách vintage",
                Brand = "Summer Style",
                BasePrice = 49.99m,
                Images = new List<string> { "/images/products/summerHat.jpg" },
                CategoryName = "Accessories"
            },
            new ProductDTO
            {
                ProductId = 103,
                Name = "Giày Sneaker Collection",
                Description = "Bộ sưu tập giày sneaker đa dạng màu sắc, phong cách trẻ trung",
                Brand = "SneakerPro",
                BasePrice = 79.99m,
                Images = new List<string> { "/images/products/sneaker.jpg" },
                CategoryName = "Footwear"
            },
            new ProductDTO
            {
                ProductId = 104,
                Name = "Áo Khoác Mùa Đông",
                Description = "Áo khoác mùa đông ấm áp, thiết kế hiện đại với mũ trùm đầu",
                Brand = "WinterWear",
                BasePrice = 199.99m,
                Images = new List<string> { "/images/products/winterCoat.jpg" },
                CategoryName = "Outerwear"
            },
            new ProductDTO
            {
                ProductId = 105,
                Name = "Quần Jeans Slim Fit",
                Description = "Quần jeans nam slim fit, chất liệu denim cao cấp",
                Brand = "DenimPro",
                BasePrice = 89.99m,
                Images = new List<string> { "/images/products/quan-jeans-slimfit-qj048-mau-xanh-16793.jpg" },
                CategoryName = "Men Clothing"
            },
            new ProductDTO
            {
                ProductId = 106,
                Name = "Áo Sơ Mi Công Sở",
                Description = "Áo sơ mi công sở thanh lịch, phù hợp môi trường làm việc",
                Brand = "OfficeStyle",
                BasePrice = 65.00m,
                Images = new List<string> { "/images/products/classTShirt.jpg" },
                CategoryName = "Men Clothing"
            },
            new ProductDTO
            {
                ProductId = 107,
                Name = "Túi Xách Nữ",
                Description = "Túi xách nữ phong cách hiện đại, chất liệu da cao cấp",
                Brand = "LuxuryBag",
                BasePrice = 150.00m,
                Images = new List<string> { "/images/products/Women_Leather_Jackets.jpg" },
                CategoryName = "Accessories"
            },
            new ProductDTO
            {
                ProductId = 108,
                Name = "Giày Chạy Bộ",
                Description = "Giày chạy bộ công nghệ cao, êm ái và bền bỉ",
                Brand = "RunFast",
                BasePrice = 120.00m,
                Images = new List<string> { "/images/products/runningShoes.jpg" },
                CategoryName = "Sportswear"
            }
        };
    }

    // Method tạo sản phẩm Top Rate hardcode
    private void CreateTopRateProducts()
    {
        TopRateProducts = new List<ProductDTO>
        {
            new ProductDTO
            {
                ProductId = 201,
                Name = "Áo Vest Nam",
                Description = "Áo vest nam công sở, thiết kế thanh lịch và chuyên nghiệp",
                Brand = "BusinessPro",
                BasePrice = 299.99m,
                Images = new List<string> { "/images/products/businessSuit.jpg" },
                CategoryName = "Formal Wear"
            },
            new ProductDTO
            {
                ProductId = 202,
                Name = "Quần Bơi Nam",
                Description = "Quần bơi nam thể thao, chất liệu nhanh khô",
                Brand = "SwimPro",
                BasePrice = 45.00m,
                Images = new List<string> { "/images/products/swimmingTrunks.jpg" },
                CategoryName = "Swimwear"
            },
            new ProductDTO
            {
                ProductId = 203,
                Name = "Áo Khoác Da Nữ",
                Description = "Áo khoác da nữ phong cách rock, chất liệu da thật",
                Brand = "RockStyle",
                BasePrice = 250.00m,
                Images = new List<string> { "/images/products/Women_Leather_Jackets.jpg" },
                CategoryName = "Outerwear"
            },
            new ProductDTO
            {
                ProductId = 204,
                Name = "Váy Dạ Hội",
                Description = "Váy dạ hội nữ thanh lịch, thiết kế độc đáo",
                Brand = "EveningGlam",
                BasePrice = 180.00m,
                Images = new List<string> { "/images/products/summerDress.jpg" },
                CategoryName = "Women Clothing"
            },
            new ProductDTO
            {
                ProductId = 205,
                Name = "Áo Hoodie Unisex",
                Description = "Áo hoodie unisex phong cách streetwear",
                Brand = "StreetStyle",
                BasePrice = 75.00m,
                Images = new List<string> { "/images/products/classTShirt.jpg" },
                CategoryName = "Casual Wear"
            },
            new ProductDTO
            {
                ProductId = 206,
                Name = "Giày Boots Nam",
                Description = "Giày boots nam phong cách rugged, chất liệu bền bỉ",
                Brand = "RuggedStyle",
                BasePrice = 160.00m,
                Images = new List<string> { "/images/products/runningShoes.jpg" },
                CategoryName = "Footwear"
            },
            new ProductDTO
            {
                ProductId = 207,
                Name = "Túi Đeo Chéo",
                Description = "Túi đeo chéo unisex, thiết kế tiện lợi",
                Brand = "CrossBag",
                BasePrice = 55.00m,
                Images = new List<string> { "/images/products/Women_Leather_Jackets.jpg" },
                CategoryName = "Accessories"
            },
            new ProductDTO
            {
                ProductId = 208,
                Name = "Áo Len Cổ Lọ",
                Description = "Áo len cổ lọ nữ, ấm áp và thời trang",
                Brand = "WarmStyle",
                BasePrice = 85.00m,
                Images = new List<string> { "/images/products/winterCoat.jpg" },
                CategoryName = "Women Clothing"
            }
        };
    }

    // Method tạo sản phẩm Sale hardcode
    private void CreateSaleProducts()
    {
        SaleProducts = new List<ProductDTO>
        {
            new ProductDTO
            {
                ProductId = 301,
                Name = "Áo Thun Basic - GIẢM 30%",
                Description = "Áo thun basic nam nữ, chất liệu cotton 100%, giảm giá 30%",
                Brand = "BasicStyle",
                BasePrice = 35.00m, // Giá sau giảm
                Images = new List<string> { "/images/products/aoThunNam.jpg" },
                CategoryName = "Casual Wear"
            },
            new ProductDTO
            {
                ProductId = 302,
                Name = "Quần Short Thể Thao - GIẢM 25%",
                Description = "Quần short thể thao nam, chất liệu thoáng mát, giảm giá 25%",
                Brand = "SportPro",
                BasePrice = 22.50m, // Giá sau giảm
                Images = new List<string> { "/images/products/swimmingTrunks.jpg" },
                CategoryName = "Sportswear"
            },
            new ProductDTO
            {
                ProductId = 303,
                Name = "Túi Xách Mini - GIẢM 40%",
                Description = "Túi xách mini nữ, thiết kế nhỏ gọn, giảm giá 40%",
                Brand = "MiniBag",
                BasePrice = 45.00m, // Giá sau giảm
                Images = new List<string> { "/images/products/Women_Leather_Jackets.jpg" },
                CategoryName = "Accessories"
            },
            new ProductDTO
            {
                ProductId = 304,
                Name = "Giày Sandal Nữ - GIẢM 35%",
                Description = "Giày sandal nữ mùa hè, thoải mái và thời trang, giảm giá 35%",
                Brand = "SummerShoes",
                BasePrice = 32.50m, // Giá sau giảm
                Images = new List<string> { "/images/products/runningShoes.jpg" },
                CategoryName = "Footwear"
            },
            new ProductDTO
            {
                ProductId = 305,
                Name = "Áo Len Cardigan - GIẢM 20%",
                Description = "Áo len cardigan nữ, ấm áp và dễ phối đồ, giảm giá 20%",
                Brand = "WarmCardigan",
                BasePrice = 68.00m, // Giá sau giảm
                Images = new List<string> { "/images/products/winterCoat.jpg" },
                CategoryName = "Women Clothing"
            },
            new ProductDTO
            {
                ProductId = 307,
                Name = "Quần Jogger Nam - GIẢM 30%",
                Description = "Quần jogger nam thể thao, thoải mái và thời trang, giảm giá 30%",
                Brand = "JoggerStyle",
                BasePrice = 42.00m, // Giá sau giảm
                Images = new List<string> { "/images/products/quan-jeans-slimfit-qj048-mau-xanh-16793.jpg" },
                CategoryName = "Men Clothing"
            },
            new ProductDTO
            {
                ProductId = 308,
                Name = "Áo Khoác Denim - GIẢM 25%",
                Description = "Áo khoác denim unisex, phong cách vintage, giảm giá 25%",
                Brand = "VintageDenim",
                BasePrice = 75.00m, // Giá sau giảm
                Images = new List<string> { "/images/products/businessSuit.jpg" },
                CategoryName = "Outerwear"
            }
        };
    }
}
