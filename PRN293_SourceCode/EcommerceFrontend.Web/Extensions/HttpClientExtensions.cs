using System.Net.Http.Json;
using System.Text.Json;

namespace EcommerceFrontend.Web.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(
            this HttpClient client,
            string? requestUri,
            TValue value,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            var content = JsonContent.Create(value, options: options);
            var request = new HttpRequestMessage(HttpMethod.Patch, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request, cancellationToken);
        }
    }
} 