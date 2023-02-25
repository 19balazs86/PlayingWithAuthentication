using Auth0.AspNetCore.Authentication;
using BlazorWASM.BFF.Auth0.OpenIDConnect.Shared.DTO;

namespace BlazorBFFOpenIDConnect.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    [HttpGet("Login")]
    public IActionResult Login(string returnUrl = "/")
    {
        var properties = new AuthenticationProperties { RedirectUri = returnUrl, IsPersistent = true };

        return Challenge(properties);
    }

    [Authorize]
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Redirect("/");
    }

    [HttpGet("UserInfo")]
    public UserInfo GetUserInfo()
    {
        return createUserInfo(User);
    }

    private static UserInfo createUserInfo(ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal?.Identity is ClaimsIdentity claimsIdentity && claimsIdentity.IsAuthenticated)
        {
            ClaimValue[] claims = claimsPrincipal.Claims.Select(c => new ClaimValue(c.Type, c.Value)).ToArray();

            return new UserInfo
            {
                IsAuthenticated = true,
                NameClaimType   = claimsIdentity.NameClaimType,
                RoleClaimType   = claimsIdentity.RoleClaimType,
                Claims          = claims
            };
        }

        return UserInfo.Anonymous;
    }
}
