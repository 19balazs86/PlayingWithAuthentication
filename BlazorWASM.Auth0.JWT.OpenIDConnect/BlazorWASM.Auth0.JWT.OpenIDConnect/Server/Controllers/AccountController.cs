using BlazorWASM.Auth0.JWT.OpenIDConnect.Shared;

namespace BlazorWASM.Auth0.JWT.OpenIDConnect.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AccountController : ControllerBase
{
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
