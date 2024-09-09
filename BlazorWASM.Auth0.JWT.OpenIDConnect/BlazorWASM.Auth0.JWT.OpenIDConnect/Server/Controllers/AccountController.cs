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

    [Authorize]
    [HttpGet("Auth0UserInfo")]
    public void GetAuth0UserInfo()
    {
        // string AccessToken = Request.Headers.Authorization!;

        // You can use the Access Token to call the user-info endpoint for more details
        // Or better to use a custom action to add the information to the token as a claim

        // curl -X 'GET' 'https://<Auth0-Domain>/userinfo' -H 'Authorization: Bearer TOKEN'
    }
}
