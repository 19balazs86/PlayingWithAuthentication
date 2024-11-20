using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorOidc.Miscellaneous.OidcRefreshToken;

/*
- The following solution refreshes the token by calling the token endpoint through the backchannel
- It stores all tokens in a cookie, increasing the cookie's size with each request
- BFF pattern could be a better approach, along with storing tokens in a database for periodic refreshes
- Every solution has its pros and cons

- Source: https://github.com/dotnet/blazor-samples/blob/main/9.0/BlazorWebAppOidc/BlazorWebAppOidc/CookieOidcRefresher.cs
- Issue: https://github.com/dotnet/aspnetcore/issues/8175

- Usage: services.ConfigureCookieOidcRefresh();
*/

public sealed class CookieOidcRefresher(IOptionsMonitor<OpenIdConnectOptions> _oidcOptionsMonitor)
{
    public async Task ValidateOrRefreshCookieAsync(CookieValidatePrincipalContext validateContext, string oidcScheme)
    {
        string? accessTokenExpirationText = validateContext.Properties.GetTokenValue("expires_at");

        if (!DateTimeOffset.TryParse(accessTokenExpirationText, out DateTimeOffset accessTokenExpiration))
        {
            return;
        }

        OpenIdConnectOptions oidcOptions = _oidcOptionsMonitor.Get(oidcScheme);

        DateTimeOffset now = oidcOptions.TimeProvider!.GetUtcNow();

        if (now + TimeSpan.FromMinutes(5) < accessTokenExpiration)
        {
            return;
        }

        Console.WriteLine("Refreshing OIDC cookie token for '{0}'", validateContext.Principal?.Identity?.Name);

        OpenIdConnectConfiguration oidcConfiguration = await oidcOptions.ConfigurationManager!.GetConfigurationAsync(validateContext.HttpContext.RequestAborted);

        string tokenEndpoint = oidcConfiguration.TokenEndpoint ?? throw new InvalidOperationException("Cannot refresh cookie. TokenEndpoint missing!");

        using var httpContent = new FormUrlEncodedContent(new Dictionary<string, string?>()
        {
            ["grant_type"]    = "refresh_token",
            ["client_id"]     = oidcOptions.ClientId,
            ["client_secret"] = oidcOptions.ClientSecret,
            ["scope"]         = string.Join(" ", oidcOptions.Scope),
            ["refresh_token"] = validateContext.Properties.GetTokenValue("refresh_token"),
        });

        using HttpResponseMessage refreshResponse = await oidcOptions.Backchannel.PostAsync(tokenEndpoint, httpContent);

        if (!refreshResponse.IsSuccessStatusCode)
        {
            validateContext.RejectPrincipal();
            return;
        }

        string refreshJson = await refreshResponse.Content.ReadAsStringAsync();
        var    message     = new OpenIdConnectMessage(refreshJson);

        TokenValidationParameters validationParameters = oidcOptions.TokenValidationParameters.Clone();

        if (oidcOptions.ConfigurationManager is BaseConfigurationManager baseConfigurationManager)
        {
            validationParameters.ConfigurationManager = baseConfigurationManager;
        }
        else
        {
            validationParameters.ValidIssuer = oidcConfiguration.Issuer;
            validationParameters.IssuerSigningKeys = oidcConfiguration.SigningKeys;
        }

        TokenValidationResult validationResult = await oidcOptions.TokenHandler.ValidateTokenAsync(message.IdToken, validationParameters);

        if (!validationResult.IsValid)
        {
            validateContext.RejectPrincipal();
            return;
        }

        JwtSecurityToken validatedIdToken = JwtSecurityTokenConverter.Convert(validationResult.SecurityToken as JsonWebToken);

        validatedIdToken.Payload["nonce"] = null;

        _oidcTokenValidator.ValidateTokenResponse(new OpenIdConnectProtocolValidationContext
        {
            ProtocolMessage  = message,
            ClientId         = oidcOptions.ClientId,
            ValidatedIdToken = validatedIdToken,
        });

        validateContext.ShouldRenew = true;

        validateContext.ReplacePrincipal(new ClaimsPrincipal(validationResult.ClaimsIdentity));

        int expiresIn = int.Parse(message.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture);

        DateTimeOffset expiresAt = now + TimeSpan.FromSeconds(expiresIn);

        AuthenticationToken[] authenticationTokens =
        [
            new() { Name = "access_token",  Value = message.AccessToken },
            new() { Name = "id_token",      Value = message.IdToken },
            new() { Name = "refresh_token", Value = message.RefreshToken },
            new() { Name = "token_type",    Value = message.TokenType },
            new() { Name = "expires_at",    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture) }
        ];

        validateContext.Properties.StoreTokens(authenticationTokens);
    }

    private readonly OpenIdConnectProtocolValidator _oidcTokenValidator = new()
    {
        // We no longer have the original nonce cookie which is deleted at the end of the authorization code flow having served its purpose.
        // Even if we had the nonce, it's likely expired. It's not intended for refresh requests. Otherwise, we'd use oidcOptions.ProtocolValidator.
        RequireNonce = false,
    };
}
