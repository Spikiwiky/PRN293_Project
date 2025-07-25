using EcommerceFrontend.Web.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Blog
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public List<BlogDto> Blogs { get; set; } = new();
        public int TotalCount { get; set; }
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public IndexModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        public async Task OnGetAsync(string? search, int? categoryId, int pageNumber = 1, int pageSize = 10)
        {
            Search = search;
            CategoryId = categoryId;
            PageNumber = pageNumber;
            PageSize = pageSize;

            var client = _httpClientFactory.CreateClient("MyAPI");
            var url = $"{_apiSettings.BaseUrl}/api/saleblog/paged?search={search}&categoryId={categoryId}&pageNumber={pageNumber}&pageSize={pageSize}";
            var result = await client.GetFromJsonAsync<PagedResultDto<BlogDto>>(url);

            if (result != null)
            {
                Blogs = result.Items.ToList();
                TotalCount = result.TotalCount;
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            await client.DeleteAsync($"{_apiSettings.BaseUrl}/api/saleblog/{id}");
            return RedirectToPage();
        }
    }
}
