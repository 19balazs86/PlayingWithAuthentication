using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace ApiJWT
{
    public static class AuthHelperWithCert
    {
        // Generate a Self-Signed Certificate with openssl
        // Or: dotnet dev-certs https --password test --export-path ./certificate.p12
        private static readonly X509Certificate2 _cert = new X509Certificate2("certificate.p12", "test");

        private static readonly string _issuer   = "http://localhost:5000";
        private static readonly string _audience = "http://localhost:5000";

        //private static SecurityKey _securityKeySymm = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));

        private static readonly SecurityKey _securityKeyX509 = new X509SecurityKey(_cert);

        // You can use the SymmetricSecurityKey with SecurityAlgorithms.HmacSha256
        private static readonly SigningCredentials _signingCredential = new SigningCredentials(_securityKeyX509, SecurityAlgorithms.RsaSha256);

        private static readonly TokenValidationParameters _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,

            ClockSkew = TimeSpan.Zero,

            ValidIssuer      = _issuer,
            ValidAudience    = _audience,
            IssuerSigningKey = _securityKeyX509
        };

        public static IServiceCollection AddJwtAuthenticationWithCert(this IServiceCollection services)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = _tokenValidationParameters;
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context => // Retrieve the token from the cookie.
                        {
                            context.Token = context.Request.Cookies["CookieName"];
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }

        public static string CreateToken(IEnumerable<Claim> claims)
        {
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject            = new ClaimsIdentity(claims),
                Issuer             = _issuer,
                Audience           = _audience,
                Expires            = DateTime.Now.AddDays(1),
                SigningCredentials = _signingCredential
            };

            SecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static bool TryValidateToken(string token, out ClaimsPrincipal? claimsPrincipal)
        {
            SecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                claimsPrincipal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var securityToken);

                return true;
            }
            catch (Exception) // The exception can be: ArgumentException, SecurityTokenValidationException
            {
                claimsPrincipal = null;

                return false;
            }
        }
    }
}
