using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ApiJWT
{
    public static class AuthHelper
    {
        private static readonly string _issuer   = "https://localhost:5000";
        private static readonly string _audience = "https://localhost:5000";

        //private static SecurityKey _securityKeySymm = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));

        private static readonly RSA _rsa;

        private static readonly RsaSecurityKey _securityKey;
        private static readonly SigningCredentials _signingCredential;

        private static readonly TokenValidationParameters _tokenValidationParameters;

        static AuthHelper()
        {
            _rsa = RSA.Create();

            // RSA can be created with the public key and the authentication works. But you can not generate tokens with the public key.
            _rsa.ImportFromPem(File.ReadAllText("Key-RSA-Private.pem"));

            _securityKey = new RsaSecurityKey(_rsa);

            // For simplicity, the SymmetricSecurityKey can be used with HmacSha256, but the symmetric key is not that safe.
            _signingCredential = new SigningCredentials(_securityKey, SecurityAlgorithms.RsaSha256);

            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer           = true,
                ValidateAudience         = true,
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,

                ClockSkew = TimeSpan.Zero,

                ValidIssuer      = _issuer,
                ValidAudience    = _audience,
                IssuerSigningKey = _securityKey
            };
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = _tokenValidationParameters;
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context => // Retrieve the token from the cookie.
                        {
                            context.Token = context.Request.Cookies["TokenCookieName"];
                            return Task.CompletedTask;
                        }
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

        public static bool TryValidateToken(string token, out ClaimsPrincipal claimsPrincipal)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                claimsPrincipal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var securityToken);

                return true;
            }
            catch (Exception) // The exception can be: SecurityTokenExpiredException, SecurityTokenValidationException
            {
                claimsPrincipal = new ClaimsPrincipal();

                return false;
            }
        }
    }
}
