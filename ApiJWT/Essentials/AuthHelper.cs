using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ApiJWT.Essentials;

public static class AuthHelper
{
    private static readonly string _issuer   = "https://localhost:5000";
    private static readonly string _audience = "https://localhost:5000";

    // private static SecurityKey _securityKeySymm = new SymmetricSecurityKey("superSecretKey_WhichIsEnoughLong@345"u8.ToArray());

    private static readonly RSA _rsa;

    private static readonly RsaSecurityKey _securityKey;
    private static readonly SigningCredentials _signingCredential;

    private static readonly TokenValidationParameters _tokenValidationParameters;

    static AuthHelper()
    {
        _rsa = RSA.Create();

        // You can import the public RSA key as well, and the authentication will work. However, the token can only be generated using the private key.
        _rsa.ImportFromPem(File.ReadAllText("Key-RSA-Private.pem"));

        _securityKey = new RsaSecurityKey(_rsa);

        // For simplicity, the SymmetricSecurityKey can be used with HmacSha256Signature, but the symmetric key is not that safe.
        _signingCredential = new SigningCredentials(_securityKey, SecurityAlgorithms.RsaSha256Signature);

        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer   = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            //ValidateIssuerSigningKey = true, // Only need for X509Certificate

            ClockSkew = TimeSpan.Zero, // Nick Chapsas explained it -> https://youtu.be/meBxWjA_2YY

            ValidIssuer      = _issuer,
            ValidAudience    = _audience,
            IssuerSigningKey = _securityKey,

            NameClaimType = UserModel.NameClaimType,
            RoleClaimType = UserModel.RoleClaimType
        };
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;

                options.TokenValidationParameters = _tokenValidationParameters;

                options.MapInboundClaims = false;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => // Retrieve the token from the cookie.
                    {
                        context.Token = context.Request.Cookies["TokenCookieName"];
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = onAuthenticationFailed,
                    OnTokenValidated       = onTokenValidated
                };
            });

        return services;
    }

    public static string CreateToken(IEnumerable<Claim> claims)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject            = new ClaimsIdentity(claims),
            Issuer             = _issuer,
            Audience           = _audience,
            Expires            = DateTime.UtcNow.AddDays(1),
            SigningCredentials = _signingCredential
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public static bool TryValidateToken(string token, out ClaimsPrincipal claimsPrincipal, out string? invalidReason)
    {
        return tryValidateToken(token, _tokenValidationParameters, out claimsPrincipal, out _, out invalidReason);
    }

    /// <summary>
    /// An expired token can be validated checking only the IssuerSigningKey
    /// </summary>
    public static bool TryValidateExpiredToken(string token, out JwtSecurityToken? jwtSecurityToken, out string? invalidReason)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer   = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            IssuerSigningKey = _securityKey
        };

        return tryValidateToken(token, tokenValidationParameters, out _, out jwtSecurityToken, out invalidReason);
    }

    private static bool tryValidateToken(
        string token,
        TokenValidationParameters tokenValidationParameters,
        out ClaimsPrincipal claimsPrincipal,
        out JwtSecurityToken? jwtSecurityToken,
        out string? invalidReason)
    {
        invalidReason    = null;
        jwtSecurityToken = null;

        var tokenHandler = new JwtSecurityTokenHandler { MapInboundClaims = false };

        try
        {
            claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            jwtSecurityToken = securityToken as JwtSecurityToken;

            return true;
        }
        catch (Exception ex) // The exception can be: SecurityTokenExpiredException, SecurityTokenValidationException
        {
            claimsPrincipal = new ClaimsPrincipal();

            invalidReason = ex.Message;

            return false;
        }
    }

    private static Task onTokenValidated(TokenValidatedContext ctx)
    {
        // IServiceProvider serviceProvider = ctx.HttpContext.RequestServices;

        string jwtId = ctx.SecurityToken.Id;

        if (!RefreshTokenRepository.IsValidToken(jwtId))
        {
            ctx.Response.Headers.Append("Reason", "Token is invalidated");

            ctx.Fail("Token is not valid anymore.");
        }

        return Task.CompletedTask;
    }

    private static Task onAuthenticationFailed(AuthenticationFailedContext ctx)
    {
        // IServiceProvider serviceProvider = ctx.HttpContext.RequestServices;

        // This is not necessary. The handler sets the header with the error description
        if (ctx.Exception.GetType() == typeof(SecurityTokenExpiredException))
            ctx.Response.Headers.Append("IsTokenExpired", "true");

        return Task.CompletedTask;
    }
}
