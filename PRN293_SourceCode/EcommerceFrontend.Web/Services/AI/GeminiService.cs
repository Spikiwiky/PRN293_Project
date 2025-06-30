using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using EcommerceFrontend.Web.Services.AI;
using Microsoft.Extensions.Options;

namespace EcommerceFrontend.Web.Service.AI
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GeminiService> _logger;
        private readonly string _systemPrompt;
        private readonly List<string> _conversationHistory;
        private string _lastResponse;

        public GeminiService(
            HttpClient httpClient,
            ILogger<GeminiService> logger,
            IOptions<GeminiSettings> geminiSettings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _conversationHistory = new List<string>();
            _lastResponse = string.Empty;

            if (geminiSettings == null || string.IsNullOrEmpty(geminiSettings.Value.ApiKey))
            {
                throw new InvalidOperationException("Gemini API Key chưa được cấu hình.");
            }
            _apiKey = geminiSettings.Value.ApiKey;

            _systemPrompt = @"
Bạn là một trợ lý AI của một nền tảng thương mại điện tử chuyên bán quần áo. 
Nhiệm vụ của bạn là gợi ý sản phẩm quần áo cụ thể, phù hợp với nhu cầu người dùng, với câu trả lời ngắn gọn (2–3 câu) và luôn đề xuất ít nhất một sản phẩm từ danh sách có sẵn (giá bằng VND).
Nếu câu hỏi chứa 'các sản phẩm bạn vừa liệt kê' hoặc 'sản phẩm vừa đề cập', chỉ gợi ý từ các sản phẩm trong phản hồi trước.
Nếu hỏi về sản phẩm không có, thông báo và gợi ý thay thế.
Nếu hỏi so sánh, nêu rõ sự khác biệt về giá, chất liệu, công dụng.
Nếu hỏi chung chung, gợi ý sản phẩm phổ biến và đặt câu hỏi để làm rõ nhu cầu.
Nếu hỏi ngoài lề, trả lời ngắn gọn và gợi ý sản phẩm quần áo phù hợp.
Phân tích đặc điểm cá nhân (giới tính, sở thích, dáng người) hoặc ngữ cảnh (dã ngoại, công sở, tiệc tùng) để gợi ý quần áo phù hợp.
";
        }

        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

        public async Task<string> GetResponseFromGemini(string userMessage)
        {
            try
            { 
                var keyCalls = new[] { "alo", "ê", "e", "ơi", "này" };
                var normalized = userMessage.Trim().ToLowerInvariant();
                if (keyCalls.Contains(normalized))
                {
                    return "Group 6 đây, xin chào quý khách cần gì";
                }
                 
                if (string.IsNullOrWhiteSpace(userMessage) || userMessage.Length < 10)
                {
                    userMessage = "Gợi ý quần áo phù hợp để mặc công sở.";
                    _logger.LogWarning("Câu hỏi người dùng quá ngắn, sử dụng câu hỏi mặc định: {UserMessage}", userMessage);
                }
                 
                _conversationHistory.Add(userMessage);
                if (_conversationHistory.Count > 5)
                {
                    _conversationHistory.RemoveAt(0);
                }
                 
                bool refersToLastResponse = Regex.IsMatch(userMessage.ToLower(), @"các sản phẩm bạn vừa liệt kê|sản phẩm vừa đề cập");
                bool isComparison = userMessage.ToLower().Contains("so sánh");
                bool isBudgetQuery = Regex.IsMatch(userMessage.ToLower(), @"dưới\s*(\d+)\.?\d*\s*vnd");
                string productContext = string.Empty;
                 
                if (refersToLastResponse && !string.IsNullOrEmpty(_lastResponse))
                {
                    productContext = _lastResponse;
                }
                else
                {
                    var products = JsonConvert.DeserializeObject<List<dynamic>>(GetProductContext());
                    if (isBudgetQuery)
                    {
                        var match = Regex.Match(userMessage, @"dưới\s*(\d+)\.?\d*\s*VND");
                        if (match.Success && int.TryParse(match.Groups[1].Value, out int budget))
                        {
                            products = products.Where(p => (int)p.Price <= budget).ToList();
                        }
                    }
                    else if (isComparison)
                    { 
                        var productNames = ExtractProductNames(userMessage);
                        products = products.Where(p => productNames.Any(name => p.Name.ToString().ToLower().Contains(name.ToLower()))).ToList();
                    }
                    else
                    { 
                        var keywords = new[] { "áo", "quần", "váy", "set", "khoác" };
                        var matchedKeyword = keywords.FirstOrDefault(k => userMessage.ToLower().Contains(k));
                        if (matchedKeyword != null)
                        {
                            products = products.Where(p => p.Name.ToString().ToLower().Contains(matchedKeyword)).ToList();
                        }
                    }
                    productContext = JsonConvert.SerializeObject(products);
                }
                 
                string fullPrompt = $"{_systemPrompt}\n\nLịch sử câu hỏi: {string.Join("; ", _conversationHistory)}";
                if (!string.IsNullOrEmpty(productContext))
                {
                    fullPrompt += $"\n\nSản phẩm có sẵn: {productContext}";
                }
                fullPrompt += $"\n\nCâu hỏi người dùng: {userMessage}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = fullPrompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 500
                    }
                };

                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ApiUrl}?key={_apiKey}")
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Phản hồi từ Gemini API: {JsonResponse}", jsonResponse);

                    try
                    {
                        dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                        if (result?.candidates != null && result.candidates.Count > 0)
                        {
                            var candidate = result.candidates[0];
                            if (candidate?.content?.parts != null && candidate.content.parts.Count > 0)
                            {
                                _lastResponse = candidate.content.parts[0].text?.ToString() ?? string.Empty;
                                return _lastResponse;
                            }
                        }
                        return "Không tìm thấy văn bản phản hồi trong candidates.";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi phân tích phản hồi từ Gemini API: {JsonResponse}", jsonResponse);
                        return $"Lỗi phân tích phản hồi: {ex.Message}";
                    }
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Yêu cầu Gemini API thất bại. Trạng thái: {StatusCode}, Chi tiết: {ErrorResponse}", response.StatusCode, errorResponse);
                    return $"Lỗi: {response.StatusCode}, Chi tiết: {errorResponse}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi Gemini API.");
                return $"Lỗi: {ex.Message}";
            }
        }

        private List<string> ExtractProductNames(string userMessage)
        {
            var products = JsonConvert.DeserializeObject<List<dynamic>>(GetProductContext());
            var productNames = products.Select(p => p.Name.ToString()).ToList();
            var matchedNames = new List<string>();
            foreach (var name in productNames)
            {
                if (userMessage.ToLower().Contains(name.ToLower()))
                {
                    matchedNames.Add(name);
                }
            }
            return matchedNames;
        }

        private string GetProductContext()
        {
            var products = new[]
            {
        new { Name = "Áo thun cotton nam cổ tròn phong cách tối giản", Price = 250000, Description = "Áo thun cotton nam với thiết kế cổ tròn đơn giản, phù hợp cho mọi dịp từ đi làm đến dạo phố. Chất liệu cotton 100% mềm mại, thấm hút mồ hôi, mang lại cảm giác thoải mái suốt cả ngày. Màu sắc đa dạng như trắng, đen, xanh navy, dễ dàng phối đồ. Size: S, M, L, XL. Hướng dẫn bảo quản: Giặt máy ở nhiệt độ thấp, tránh dùng nước nóng hoặc chất tẩy mạnh. Sản phẩm được may thủ công tại Việt Nam, thân thiện với môi trường.", Category = "Quần áo" },
        new { Name = "Đầm maxi nữ voan hoa nhí thanh lịch", Price = 450000, Description = "Đầm maxi nữ làm từ chất liệu voan cao cấp, họa tiết hoa nhí tinh tế, tôn lên vẻ đẹp nhẹ nhàng và nữ tính. Thiết kế dài qua gối, phù hợp cho các buổi tiệc tối hoặc đi chơi. Có dây lưng kèm theo để điều chỉnh eo, size free size. Màu sắc: Hồng pastel, xanh lá, trắng ngọc. Bảo quản: Giặt tay với nước lạnh, phơi ngược để giữ form dáng. Sản phẩm được sản xuất tại Việt Nam, đảm bảo chất lượng và độ bền cao.", Category = "Quần áo" },
        new { Name = "Áo sơ mi nữ lụa phối ren sang trọng", Price = 380000, Description = "Áo sơ mi nữ làm từ lụa tự nhiên, kết hợp phần tay và cổ áo bằng ren tinh xảo, mang phong cách sang trọng và hiện đại. Phù hợp mặc đi làm, dự tiệc hoặc kết hợp cùng quần jeans. Size: S, M, L. Màu sắc: Trắng, be, đen. Hướng dẫn bảo quản: Giặt tay nhẹ nhàng, ủi ở nhiệt độ thấp, tránh ánh nắng trực tiếp. Sản phẩm thủ công, hỗ trợ bền vững môi trường.", Category = "Quần áo" },
        new { Name = "Quần jeans nam ống suông thời thượng", Price = 320000, Description = "Quần jeans nam ống suông, chất liệu denim co giãn nhẹ, phù hợp với mọi vóc dáng. Thiết kế tối giản với đường may chắc chắn, độ bền cao. Màu sắc: Xanh đậm, xám, đen. Size: 29, 30, 32, 34. Phù hợp mặc hàng ngày hoặc kết hợp với áo thun. Bảo quản: Giặt máy ở chế độ nhẹ, lộn trái khi giặt. Sản phẩm từ làng nghề Việt Nam, thân thiện môi trường.", Category = "Quần áo" },
        new { Name = "Áo khoác bomber unisex phong cách Hàn Quốc", Price = 550000, Description = "Áo khoác bomber unisex làm từ vải dù chống thấm nước, thiết kế phong cách Hàn Quốc với cổ lông và túi hai bên. Phù hợp cho mùa thu đông, dễ phối đồ với quần jeans hoặc quần jogger. Size: S, M, L, XL. Màu sắc: Đen, xanh navy, xám. Bảo quản: Giặt máy ở chế độ nhẹ, phơi khô tự nhiên. Sản phẩm thủ công, góp phần bảo vệ môi trường.", Category = "Quần áo" },
        new { Name = "Chân váy xòe vải lanh nữ phong cách vintage", Price = 290000, Description = "Chân váy xòe làm từ vải lanh tự nhiên, mang phong cách vintage với độ dài qua gối. Phù hợp mặc đi làm hoặc dạo phố, kết hợp với áo sơ mi hoặc áo thun. Size: Free size, có dây rút điều chỉnh. Màu sắc: Be, xanh olive, nâu đất. Bảo quản: Giặt tay, phơi khô tự nhiên, tránh vò mạnh. Sản phẩm thủ công Việt Nam, an toàn cho người dùng.", Category = "Quần áo" },
        new { Name = "Áo hoodie nữ in họa tiết hoạt hình", Price = 300000, Description = "Áo hoodie nữ làm từ cotton pha, in họa tiết hoạt hình dễ thương, phù hợp cho giới trẻ. Thiết kế cổ trùm đầu, túi kangaroo, giữ ấm tốt. Size: S, M, L. Màu sắc: Xanh lá, hồng, xám. Bảo quản: Giặt máy ở nhiệt độ thấp, phơi ngược để giữ màu. Sản phẩm thân thiện môi trường, sản xuất tại Việt Nam.", Category = "Quần áo" },
        new { Name = "Quần short kaki nam ống rộng", Price = 220000, Description = "Quần short kaki nam ống rộng, chất liệu kaki thoáng mát, phù hợp cho mùa hè. Thiết kế tối giản, có dây rút và túi hai bên. Size: 29, 30, 32, 34. Màu sắc: Khaki, xanh navy, đen. Bảo quản: Giặt máy nhẹ, phơi khô tự nhiên. Sản phẩm từ làng nghề thủ công, bền đẹp theo thời gian.", Category = "Quần áo" },
        new { Name = "Áo len cổ lọ nữ mùa đông", Price = 400000, Description = "Áo len cổ lọ nữ làm từ len cao cấp, giữ ấm tốt, phù hợp cho mùa đông lạnh. Thiết kế ôm vừa vặn, dễ phối với quần jeans hoặc váy. Size: S, M, L. Màu sắc: Xám, be, đỏ burgundy. Bảo quản: Giặt tay nhẹ, phơi khô tự nhiên, tránh máy sấy. Sản phẩm thủ công, thân thiện môi trường.", Category = "Quần áo" },
        new { Name = "Set đồ thể thao nam vải thun co giãn", Price = 480000, Description = "Set đồ thể thao nam gồm áo thun và quần short, làm từ vải thun co giãn, thấm hút mồ hôi. Phù hợp tập gym, chạy bộ hoặc mặc hàng ngày. Size: S, M, L, XL. Màu sắc: Đen, xanh dương, xám. Bảo quản: Giặt máy ở chế độ nhẹ, phơi khô tự nhiên. Sản phẩm Việt Nam, an toàn sức khỏe.", Category = "Quần áo" }
            };
            return JsonConvert.SerializeObject(products);
        }
    }
}