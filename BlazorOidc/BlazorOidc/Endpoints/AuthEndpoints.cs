using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BlazorOidc.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/auth");

        group.MapGet("/login",   login);
        group.MapPost("/logout", logout).RequireAuthorization();

        return app;
    }

    private static ChallengeHttpResult login(string? returnUrl)
    {
        var authProperties = new AuthenticationProperties
        {
            RedirectUri  = getRedirectUrl(returnUrl),
            IsPersistent = true,
            ExpiresUtc   = DateTimeOffset.UtcNow.AddDays(7)
        };

        return TypedResults.Challenge(authProperties);
    }

    private static SignOutHttpResult logout([FromForm] string? returnUrl) // [FromForm] attribute compels the middleware to check the anti-forgery token
    {
        // Ignore returnUrl, as only the base URL is defined in Auth0 'Allowed Logout URLs'
        var authProperties = new AuthenticationProperties { RedirectUri = getRedirectUrl(null) };

        string[] authSchemes = [CookieAuthenticationDefaults.AuthenticationScheme, Program.AuthOidcScheme];

        return TypedResults.SignOut(authProperties, authSchemes);
    }

    private static string getRedirectUrl(string? returnUrl)
    {
        const string pathBase = "/";

        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return pathBase;
        }

        if (Uri.IsWellFormedUriString(returnUrl, UriKind.Absolute))
        {
            return new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
        }

        return returnUrl.StartsWith('/') ? returnUrl : $"{pathBase}{returnUrl}";
    }
}
