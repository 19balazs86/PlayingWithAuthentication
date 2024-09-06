using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace BlazorWASM.Auth0.JWT.OpenIDConnect.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ValidateTokenController : ControllerBase
{
    // The following JSON can be get from: <YourAuthority>/.well-known/jwks.json
    private const string _jwksJson = """
        {
          "keys": [
            {
              "alg": "RS256",
              "kty": "RSA",
              "use": "sig",
              "n": "9kvvdDdC50_Jm1KaintWpjif62LhkcD334LWpLC3Nj1uM0KVQn5buNPnPw4MFzfz9QcO7c8X1mYMmccqrMIJ1qybMggzPTW9cEsVn49WAEINKOE2m5MbL9zBB3m84BPB3XkPQrwwQWZ6Ob_CGijUQwa63-savQg_v7y8Uxwzs9BLbVMG06O8gFeNqkGkywVjQXQTPLqQImK8M50YpUycKG79vhknvTE_9xvPb6d_M68mAUYbammrcGavoA3K6U_IYXvlcWxCRS9L_ArIFdssWOGc1Pdmxl_7_Hi3oyDhkdX7TYX5jExhT37WQNIb7ViLNftjgaEoME_xXugP0Zwj5Q",
              "e": "AQAB",
              "kid": "HiDgsujlHtNEwVGZNWV67",
              "x5t": "jZQ831VwwPCsGOxNyGofl61OA38",
              "x5c": [
                "MIIDHzCCAgegAwIBAgIJBzdKV7U70f6nMA0GCSqGSIb3DQEBCwUAMC0xKzApBgNVBAMTIjE5YmFsYXpzODYtZGV2LXRlbmFudC5ldS5hdXRoMC5jb20wHhcNMjMwMjIzMTQ1NzQ0WhcNMzYxMTAxMTQ1NzQ0WjAtMSswKQYDVQQDEyIxOWJhbGF6czg2LWRldi10ZW5hbnQuZXUuYXV0aDAuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA9kvvdDdC50/Jm1KaintWpjif62LhkcD334LWpLC3Nj1uM0KVQn5buNPnPw4MFzfz9QcO7c8X1mYMmccqrMIJ1qybMggzPTW9cEsVn49WAEINKOE2m5MbL9zBB3m84BPB3XkPQrwwQWZ6Ob/CGijUQwa63+savQg/v7y8Uxwzs9BLbVMG06O8gFeNqkGkywVjQXQTPLqQImK8M50YpUycKG79vhknvTE/9xvPb6d/M68mAUYbammrcGavoA3K6U/IYXvlcWxCRS9L/ArIFdssWOGc1Pdmxl/7/Hi3oyDhkdX7TYX5jExhT37WQNIb7ViLNftjgaEoME/xXugP0Zwj5QIDAQABo0IwQDAPBgNVHRMBAf8EBTADAQH/MB0GA1UdDgQWBBQvKIKJvqMZRQNjDSxOvhAABnqhjTAOBgNVHQ8BAf8EBAMCAoQwDQYJKoZIhvcNAQELBQADggEBAGjU703Oz0R/42koRJLYgCajXHGXIXIuLky6jfIjlzfNjezkRIjK82yDy1xYm0qOENIuM7i0T8PVtGbFkrqLzYhCdgndImJepThzp7FiBuWlhOE4wwMsA8E8yxrsIttR2SB2uGiSEbYHWKnZ9Uo4sNp5Br0zu5PUrf9/q6KTprYbNQ4ckf6pTwMsla8O7ryn+dqcLOaAZHqtrT9NBP34w8V04mk+hpamFPNpAxvqrz8+LllgVv4mXpg3HrWzog9o3PnQfp5kQg6ISPPihkcErTratm5mK/vudp0Y/ESMj3eYli5yL4a6VucTW/cXYVWRcxgfjm3o9ye+Y7hxQg8vTmg="
              ]
            },
            {
              "alg": "RS256",
              "kty": "RSA",
              "use": "sig",
              "n": "51Q0YiqYY65n5NtldW6hCFx5bVn5cozuOlYz8UFUZTFhHQEnAmr-m8yTlCG5LruXn_vN1IC308_Jzoo7pUsoowZDgKY8-4rwdH5-Zl16ytGl3AR46sOhzoSHZDU_7N4CHpwOqyOB2WduQdG5Pm4_60lynC5XQNiQgzta_SU_g4BgHokn7YMYwhvDPzUIfxz-MAjGb22Yn3oay7t7FEoev7yikcoOxtiEfVaO89YZXg7OvpNjHkjsUxP9NMSqFd-GDhneHnQUgpeti4c2ZwqcVlPFE_UnkVvbxqH6vaCmVKzZubBQNCI9fdD8215QEDhpx1LVs4e5eaW9KUJiVAy6yw",
              "e": "AQAB",
              "kid": "xKcESa8JMGT0fPEaWmPth",
              "x5t": "LItA08wiGfx7EH9AyJc6mqRxQbk",
              "x5c": [
                "MIIDHzCCAgegAwIBAgIJF2Mi9xfhZzYeMA0GCSqGSIb3DQEBCwUAMC0xKzApBgNVBAMTIjE5YmFsYXpzODYtZGV2LXRlbmFudC5ldS5hdXRoMC5jb20wHhcNMjMwMjIzMTQ1NzQ0WhcNMzYxMTAxMTQ1NzQ0WjAtMSswKQYDVQQDEyIxOWJhbGF6czg2LWRldi10ZW5hbnQuZXUuYXV0aDAuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA51Q0YiqYY65n5NtldW6hCFx5bVn5cozuOlYz8UFUZTFhHQEnAmr+m8yTlCG5LruXn/vN1IC308/Jzoo7pUsoowZDgKY8+4rwdH5+Zl16ytGl3AR46sOhzoSHZDU/7N4CHpwOqyOB2WduQdG5Pm4/60lynC5XQNiQgzta/SU/g4BgHokn7YMYwhvDPzUIfxz+MAjGb22Yn3oay7t7FEoev7yikcoOxtiEfVaO89YZXg7OvpNjHkjsUxP9NMSqFd+GDhneHnQUgpeti4c2ZwqcVlPFE/UnkVvbxqH6vaCmVKzZubBQNCI9fdD8215QEDhpx1LVs4e5eaW9KUJiVAy6ywIDAQABo0IwQDAPBgNVHRMBAf8EBTADAQH/MB0GA1UdDgQWBBQW6eECb3tArrsoagMODUt4FrInsDAOBgNVHQ8BAf8EBAMCAoQwDQYJKoZIhvcNAQELBQADggEBAAHMvHaHEnltveiEI3Bv3Q5gcUPyvacz9WojgHyxxtOCRql119LCA23Ayy0GvKweE42+t5nOAZ462h1ZAju3m2x1mWXBPF9Yb9D52lHeilU++fTwqELXpYpbWdXZ10E+KRn3NRnz4PhSa5gDbEPTe/rV2eIFPePLE4P/IDFKAEzAl+0K3RTjyUGFcPfaXAVHnn86Ct1HEn96GsMZpQtUUyuxZEnEbKyHxgwE0kGjciicZLA7A1xGduHBYKhy8nn6ZRfJ5Ne+638/lXIh+PnIc84qYQRhFJ2ZDO2w9+9SOqAp02/rLXQ2PPymY7ghLhzYg+N+KYGsr0M4yxUHE/XJ/4k="
              ]
            }
          ]
        }
        """;

    private readonly OidcConfig _oidcConfig;

    public ValidateTokenController(OidcConfig oidcConfig)
    {
        _oidcConfig = oidcConfig;
    }

    /// <summary>
    /// Validate the given JWT token manually
    /// </summary>
    [HttpGet]
    public IActionResult Validate(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("The 'token' is not present in the query.");

        JwtSecurityToken? jwtSecurityToken = null;

        try
        {
            jwtSecurityToken = new JwtSecurityToken(token);
        }
        catch (Exception ex)
        {
            return UnprocessableEntity("The given token is malformatted: " + ex.Message);
        }

        var jwks = new JsonWebKeySet(_jwksJson);

        JsonWebKey? jwk = jwks.Keys.SingleOrDefault(x => x.Kid.Equals(jwtSecurityToken.Header.Kid));

        if (jwks is null)
            return StatusCode(StatusCodes.Status500InternalServerError, "JsonWebKey was not found.");

        var validationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer   = true,
            ValidateLifetime = true,

            IssuerSigningKey = jwk,
            ValidAudience    = _oidcConfig.Audience,
            ValidIssuers     = _oidcConfig.ValidIssuers(),
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok("Token is valid.");
    }
}
