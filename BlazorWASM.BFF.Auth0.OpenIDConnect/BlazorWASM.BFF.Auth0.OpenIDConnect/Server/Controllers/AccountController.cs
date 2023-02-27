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

        return Challenge(properties, Auth0Constants.AuthenticationScheme);
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
        return UserInfo.FromClaimsPrincipal(User);
    }

    [Authorize(Roles = "TestRole")]
    [HttpGet("TestRole")]
    public string TestRole()
    {
        return "You have TestRole";
    }
}
