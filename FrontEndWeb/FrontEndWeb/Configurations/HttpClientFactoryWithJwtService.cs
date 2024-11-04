using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace FrontEndWeb.Configurations
{
    public interface IHttpClientFactoryWithJwtService
    {
        HttpClient CreateClientWithJwt();
    }
    public class HttpClientFactoryWithJwtService : IHttpClientFactoryWithJwtService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ApiSettings _apiSettings;

        public HttpClientFactoryWithJwtService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IOptions<ApiSettings> apiSettings
            )
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _apiSettings = apiSettings.Value;
        }

        public HttpClient CreateClientWithJwt()
        {
            var httpClient = _httpClientFactory.CreateClient();

            //Adds Jwt if it exists to the Authorization Header
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                var jwtTokenClaim = context.User.Claims.FirstOrDefault(c => c.Type == "jwt");
                if (jwtTokenClaim != null)
                {
                    var jwtToken = jwtTokenClaim.Value;
                    if (!string.IsNullOrEmpty(jwtToken))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                }
            }

            //Defines base Url
            httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl);

            return httpClient;
        }
    }
}
