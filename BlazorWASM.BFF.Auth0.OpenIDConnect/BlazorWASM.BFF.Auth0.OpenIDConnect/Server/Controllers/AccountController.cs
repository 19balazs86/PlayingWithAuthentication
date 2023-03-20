using Auth0.AspNetCore.Authentication;
using BlazorWASM.BFF.Auth0.OpenIDConnect.Shared.DTO;

namespace BlazorBFFOpenIDConnect.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AccountController : ControllerBase
{
    [HttpGet("Login")]
    public IActionResult Login(string returnUrl = "/")
    {
        var properties = new AuthenticationProperties { RedirectUri = returnUrl, IsPersistent = true };

        return Challenge(properties, Auth0Constants.AuthenticationScheme);
    }

    [Authorize]
    [HttpGet("Logout")]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // This will redirect the call to Auth0 logout as well.
        // OpenIdConnectEvents.OnRedirectToIdentityProviderForSignOut is defined when using the Auth0 package.
        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme);
    }

    [HttpGet("UserInfo")]
    public UserInfo GetUserInfo()
    {
        return UserInfo.FromClaimsPrincipal(User);
    }

    [Authorize(Roles = "TestRole")]
    [HttpGet("TestRole")]
    public string TestRole()
    {
        return "You have TestRole";
    }
}
