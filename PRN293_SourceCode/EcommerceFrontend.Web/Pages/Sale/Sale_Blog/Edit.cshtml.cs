using EcommerceFrontend.Web.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace EcommerceFrontend.Web.Pages.Sale.Sale_Blog
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        [BindProperty]
        public BlogUpdateDto Blog { get; set; } = new();

        [BindProperty]
        public IFormFile? UploadImage { get; set; }

        public EditModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            Blog = await client.GetFromJsonAsync<BlogUpdateDto>($"{_apiSettings.BaseUrl}/api/saleblog/{id}");
            if (Blog == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
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
            content.Add(new StringContent(Blog.IsPublished.ToString()), "IsPublished");

            // Gửi RemoveImage = true nếu người dùng xóa link ảnh (Blog.BlogImage == null hoặc "")
            bool removeImage = string.IsNullOrWhiteSpace(Blog.BlogImage) && (UploadImage == null || UploadImage.Length == 0);
            content.Add(new StringContent(removeImage.ToString()), "RemoveImage");

            if (UploadImage != null && UploadImage.Length > 0)
            {
                var fileContent = new StreamContent(UploadImage.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(UploadImage.ContentType);
                content.Add(fileContent, "ImageFile", UploadImage.FileName);
            }

            var response = await client.PutAsync($"{_apiSettings.BaseUrl}/api/saleblog/{id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError("", "Không thể cập nhật blog");
            return Page();
        }


    }
}
