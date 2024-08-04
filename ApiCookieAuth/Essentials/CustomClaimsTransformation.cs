using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ApiCookieAuth.Essentials;

/* You can modify the ClaimsPrincipal with this transformation, triggered after the authenticaion process.
 * You can achieve it with authentication methods like Cookie or JWT via their events mechanism.
 * The benefit of this approach is that you are independent of the authentication events.
 *
 * Milan newsletter: https://www.milanjovanovic.tech/blog/master-claims-transformation-for-flexible-aspnetcore-authorization
 * Milan video: https://youtu.be/cgjifZF8ZME
 */
public sealed class CustomClaimsTransformation : IClaimsTransformation
{
    // You can inject services from DI

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        Claim[] claims = [new Claim("CustomType", "CustomValue")];

        var claimsIdentity = new ClaimsIdentity(claims);

        principal.AddIdentity(claimsIdentity);

        return Task.FromResult(principal);
    }
}
