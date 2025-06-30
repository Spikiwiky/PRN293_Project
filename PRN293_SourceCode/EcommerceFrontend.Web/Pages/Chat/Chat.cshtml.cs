using EcommerceFrontend.Web.Service.AI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceFrontend.Web.Pages.Chat
{
    public class ChatModel : PageModel
    {
        private readonly GeminiService _geminiService;
        private readonly ILogger<ChatModel> _logger;

        [BindProperty]
        public string UserInput { get; set; }

        public string AIResponse { get; set; }

        public ChatModel(GeminiService geminiService, ILogger<ChatModel> logger)
        {
            _geminiService = geminiService;
            _logger = logger;
        }
        public void OnGet()
        { 
            AIResponse = null;
        }

        public async Task<IActionResult> OnPostAsync([FromBody] ChatRequest request)
        {
            _logger.LogInformation("Received POST request with UserInput: {UserInput}", request?.UserInput);

            if (string.IsNullOrEmpty(request?.UserInput))
            {
                _logger.LogWarning("UserInput is empty.");
                 
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var badRequestResponse = new { aiResponse = "Vui lòng nhập nội dung." };
                    return new JsonResult(badRequestResponse) { StatusCode = 400 };  
                }

                return Page();  
            }

            try
            { 
                AIResponse = await _geminiService.GetResponseFromGemini(request.UserInput);
                _logger.LogInformation("Gemini AI Response: {AIResponse}", AIResponse);
                 
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var jsonResponse = new { aiResponse = AIResponse };
                    return new JsonResult(jsonResponse);  
                }

                return Page(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API.");
                var errorResponse = $"Lỗi khi gọi Gemini API: {ex.Message}";
                 
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var jsonErrorResponse = new { aiResponse = errorResponse };
                    return new JsonResult(jsonErrorResponse) { StatusCode = 500 }; 
                }

                AIResponse = errorResponse;   
                return Page();
            }
        }
    }

    public class ChatRequest
    {
        public string UserInput { get; set; }
    }
}
