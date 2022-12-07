using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace ApiCookieAuth
{
    public sealed class ApiJwtClientAuthHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiJwtClientAuthHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext
                ?? throw new ArgumentNullException("HttpContext");

            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                string token = await _httpContextAccessor.HttpContext?.GetTokenAsync(ApiJwtClient.AccessTokenName);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

    public sealed class ApiJwtClient : IApiJwtClient
    {
        public const string AccessTokenName = "access_token";

        private readonly HttpClient _httpClient;

        public ApiJwtClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthToken> GetAuthTokenAsync(LoginRequest loginRequest)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/Auth/Login", loginRequest);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<AuthToken>();
        }

        public async Task<UserModel> GetUserModelAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/Auth");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UserModel>();
        }
    }

    public interface IApiJwtClient
    {
        Task<AuthToken> GetAuthTokenAsync(LoginRequest loginRequest);
        Task<UserModel> GetUserModelAsync();
    }
}
