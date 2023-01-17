using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace ApiCookieAuth.Controllers;

[Authorize]
[ApiController]
public class ApiJwtCallerController : ControllerBase
{
    private const string _cookieAuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    private readonly IApiJwtClient _apiJwtClient;

    public ApiJwtCallerController(IApiJwtClient apiJwtClient)
    {
        _apiJwtClient = apiJwtClient;
    }

    [HttpGet("/")]
    public async Task<ActionResult<UserModel>> Index()
    {
        UserModel userModel = await _apiJwtClient.GetUserModel();

        if (userModel is null)
            return Problem(title: "Failed to get user details", statusCode: Status500InternalServerError);

        return Ok(userModel);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return Redirect("/");
    }

    [HttpGet("Blacklist")]
    public async Task<IActionResult> AddToBlacklist()
    {
        Program.CookieBlackList.Add(User.FindFirstValue(Program.SessionClaimName));

        return Ok("You added yourself to the blacklist.");
    }

    [HttpPost("CallApi/Login")]
    [AllowAnonymous]
    public async Task<IActionResult> CallApiLogin(LoginRequest loginRequest)
    {
        AuthToken authToken = await _apiJwtClient.Login(loginRequest);

        if (authToken is null)
            return Problem(title: "Failed to log in to ApiJWT", statusCode: Status400BadRequest);

        var claims = new Claim[]
        {
            new Claim(Program.SessionClaimName,  Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, loginRequest.Name)
        };

        var claimsIdentity = new ClaimsIdentity(claims, _cookieAuthScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var properties = new AuthenticationProperties
        {
            //IsPersistent = true, // It keeps the cookie on the client side, otherwise it is a session cookie
            //ExpiresUtc = DateTime.UtcNow.AddDays(15), // Set by ExpireTimeSpan when AddCookie
            RedirectUri = "/"
        };

        var tokens = new[]
        {
            new AuthenticationToken { Name = ApiJwtClient.AccessTokenName, Value = authToken.Token }
        };

        properties.StoreTokens(tokens);

        await HttpContext.SignInAsync(_cookieAuthScheme, claimsPrincipal, properties);

        return Ok();
    }
}
