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

            // If the token is expired use the refresh token to obtain a new token.
            // Use the httpContext.SignInAsync method to save it in the cookie on the client side.

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
