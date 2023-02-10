using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ApiKeyAuth.Solutions;

public static class TestAuthenticationExtensions
{
    public static IServiceCollection ApiKeyAuthentication(this IServiceCollection services, Action<ApiKeyAuthOptions> configureOptions = null)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = Consts.AuthScheme;
            options.DefaultChallengeScheme    = Consts.AuthScheme;
        })
        .AddScheme<ApiKeyAuthOptions, ApiKeyAuthHandler>(Consts.AuthScheme, configureOptions);

        return services;
    }
}

public sealed class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthOptions>
{
    private readonly IApiKeyValidator _apiKeyValidator;

    public ApiKeyAuthHandler(
        IOptionsMonitor<ApiKeyAuthOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IApiKeyValidator apiKeyValidator) : base(options, logger, encoder, clock)
    {
        _apiKeyValidator = apiKeyValidator;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        bool isValid = _apiKeyValidator.Validate(Request);

        if (!isValid)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, "<ReplaceWith-Api-Key-FromHeader>") };

        var claimsIdentity = new ClaimsIdentity(claims, Consts.AuthScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authenticationTicket = new AuthenticationTicket(claimsPrincipal, Consts.AuthScheme);

        return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
    }
}

public sealed class ApiKeyAuthOptions : AuthenticationSchemeOptions { }

file class Consts
{
    public const string AuthScheme = "ApiKeyAuth";
}
