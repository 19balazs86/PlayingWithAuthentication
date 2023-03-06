using Auth0.AspNetCore.Authentication;
using BlazorWASM.BFF.Auth0.OpenIDConnect.Shared.DTO;

namespace BlazorBFFOpenIDConnect.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AccountController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [Authorize]
    [HttpGet("LogoutUrl")]
    public string GetLogoutUrl()
    {
        string domain   = _configuration.GetValue<string>("Authentication:Auth0:Domain")!;
        string clientId = _configuration.GetValue<string>("Authentication:Auth0:ClientId")!;

        return $"https://{domain}/v2/logout?client_id={clientId}";
    }

    [HttpGet("Login")]
    public IActionResult Login(string returnUrl = "/")
    {
        var properties = new AuthenticationProperties { RedirectUri = returnUrl, IsPersistent = true };

        return Challenge(properties, Auth0Constants.AuthenticationScheme);
    }

    [Authorize]
    [Route("Logout")]
    [HttpPost, HttpGet]
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
