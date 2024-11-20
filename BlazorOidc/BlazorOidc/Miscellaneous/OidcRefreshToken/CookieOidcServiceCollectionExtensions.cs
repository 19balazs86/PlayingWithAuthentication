using BlazorOidc;
using BlazorOidc.Miscellaneous.OidcRefreshToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Microsoft.Extensions.DependencyInjection;

public static class CookieOidcServiceCollectionExtensions
{
    public static void ConfigureCookieOidcRefresh(this IServiceCollection services)
    {
        services.AddSingleton<CookieOidcRefresher>();

        services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme).Configure(cookieOptions =>
        {
            cookieOptions.Events.OnValidatePrincipal = onValidatePrincipal;
        });

        services.AddOptions<OpenIdConnectOptions>(Program.AuthOidcScheme).Configure(oidcOptions =>
        {
            oidcOptions.Scope.Add(OpenIdConnectScope.OfflineAccess); // Request a refresh token

            oidcOptions.SaveTokens = true; // Store the refresh token
        });
    }

    private static async Task onValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var cookieOidcRefresher = context.HttpContext.RequestServices.GetRequiredService<CookieOidcRefresher>();

        await cookieOidcRefresher.ValidateOrRefreshCookieAsync(context, Program.AuthOidcScheme);
    }
}
