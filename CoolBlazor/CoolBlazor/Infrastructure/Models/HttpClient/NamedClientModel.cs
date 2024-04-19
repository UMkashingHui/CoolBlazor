using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoolBlazor.Infrastructure.Models.HttpClient
{
    public class NamedClientModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public NamedClientModel(IHttpClientFactory httpClientFactory) =>
            _httpClientFactory = httpClientFactory;

        public async Task<Stream> OnGet()
        {
            var httpClient = _httpClientFactory.CreateClient("ToolBoxApi");
            var httpResponseMessage = await httpClient.GetAsync(
                "HealthCheck");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();
                return contentStream;
            }

            return Stream.Null;
        }
    }
}