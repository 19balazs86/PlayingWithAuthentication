using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi_EF_Identity.Data;

namespace WebApi_EF_Identity.Controllers;

[ApiController]
[Route("[controller]")]
public class PasswordlessLoginController : ControllerBase
{
    private readonly UserManager<MyIdentityUser> _userManager;
    private readonly SignInManager<MyIdentityUser> _signInManager;

    public PasswordlessLoginController(UserManager<MyIdentityUser> userManager, SignInManager<MyIdentityUser> signInManager)
    {
        _userManager   = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("Create/{email}")]
    public async Task<IActionResult> CreatePasswordlessLogin(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return BadRequest("Invalid email");

        if (!await _signInManager.CanSignInAsync(user))
            return BadRequest("User can not sign in");

        string token = await _userManager.GenerateUserTokenAsync(user, PasswordlessLoginTokenProviderOptions.ProviderName, PasswordlessLoginTokenProviderOptions.Purpose);

        string passwordlessLogin = Url.Action(nameof(Login), "PasswordlessLogin", new { token, email }, Request.Scheme);

        return Ok(new { passwordlessLogin });
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> Login([FromRoute] string email, [FromQuery] string token)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return BadRequest("Invalid email");

        if (!await _signInManager.CanSignInAsync(user))
            return BadRequest("User can not sign in");

        if (_signInManager.IsSignedIn(User))
            return Ok("You are already signed in");

        var isValid = await _userManager.VerifyUserTokenAsync(user, PasswordlessLoginTokenProviderOptions.ProviderName, PasswordlessLoginTokenProviderOptions.Purpose, token);

        if (!isValid)
            return BadRequest("Invalid token");

        // await _userManager.UpdateSecurityStampAsync(user);

        await _signInManager.SignInAsync(user, isPersistent: true);

        return Ok("You are signed in");
    }
}
