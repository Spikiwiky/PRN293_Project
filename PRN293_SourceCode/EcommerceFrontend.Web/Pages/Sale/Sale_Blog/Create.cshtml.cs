using EcommerceFrontend.Web.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Blog
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        [BindProperty]
        public BlogCreateFormDto Blog { get; set; } = new();

        [BindProperty]
        public IFormFile? UploadImage { get; set; }

        public CreateModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var client = _httpClientFactory.CreateClient("MyAPI");

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(Blog.BlogCategoryId?.ToString() ?? ""), "BlogCategoryId");
            content.Add(new StringContent(Blog.BlogTittle ?? ""), "BlogTittle");
            content.Add(new StringContent(Blog.Tags ?? ""), "Tags");
            content.Add(new StringContent(Blog.BlogContent ?? ""), "BlogContent");
            content.Add(new StringContent(Blog.BlogSummary ?? ""), "BlogSummary");
            content.Add(new StringContent(Blog.PublishedDate?.ToString("o") ?? ""), "PublishedDate");
            content.Add(new StringContent(Blog.IsPublished.ToString()), "IsPublished");

            if (UploadImage != null && UploadImage.Length > 0)
            {
                var fileContent = new StreamContent(UploadImage.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(UploadImage.ContentType);
                content.Add(fileContent, "ImageFile", UploadImage.FileName);
            }
            else if (!string.IsNullOrWhiteSpace(Blog.BlogImage))
            {
                // Gửi URL ảnh nếu người dùng nhập URL
                content.Add(new StringContent(Blog.BlogImage), "ImageUrl");
            }

            var response = await client.PostAsync($"{_apiSettings.BaseUrl}/api/saleblog", content);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError("", "Không thể thêm blog");
            return Page();
        }
    }
}
