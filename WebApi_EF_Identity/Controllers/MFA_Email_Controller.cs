using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi_EF_Identity.Data;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WebApi_EF_Identity.Controllers;

[Authorize]
[ApiController]
[Route("2FA-Email")]
public sealed class MFA_Email_Controller : ControllerBase
{
    private readonly UserManager<MyIdentityUser> _userManager;
    private readonly SignInManager<MyIdentityUser> _signInManager;

    private readonly string _tokenProvider;

    public MFA_Email_Controller(UserManager<MyIdentityUser> userManager, SignInManager<MyIdentityUser> signInManager)
    {
        _userManager   = userManager;
        _signInManager = signInManager;

        _tokenProvider = TokenOptions.DefaultEmailProvider;
    }

    [AllowAnonymous]
    [HttpPost("Login/{code}")]
    public async Task<IActionResult> Login(string code)
    {
        // In order to make 2FA work, you need to log in with username and password to create the 'Identity.TwoFactorUserId' cookie.
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        if (user is null)
            return Unauthorized("First you need to login with user name and password");

        SignInResult result = await _signInManager.TwoFactorSignInAsync(_tokenProvider, code, isPersistent: true, rememberClient: true);

        if (result.Succeeded)
            return Ok("You are signed in");

        return Problem(title: "Failed to login", detail: result.ToString(), statusCode: StatusCodes.Status400BadRequest);
    }

    [HttpGet("setup")]
    public async Task<IActionResult> GetSetup()
    {
        //string email = User.Identity.Name;

        var user = await _userManager.GetUserAsync(User);

        bool isTwoFactorAuthEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

        string code = await _userManager.GenerateTwoFactorTokenAsync(user, _tokenProvider);

        var response = new
        {
            TwoFactorAuthEnabled = isTwoFactorAuthEnabled,
            code
        };

        return Ok(response);
    }

    [HttpPost("setup/{code}")]
    public async Task<IActionResult> Enable(string code)
    {
        var user = await _userManager.GetUserAsync(User);

        bool isValidCode = await _userManager.VerifyTwoFactorTokenAsync(user, _tokenProvider, code);

        if (!isValidCode)
            return BadRequest("Invalid code");

        await _userManager.SetTwoFactorEnabledAsync(user, enabled: true);

        return Ok("Two-Factor authentication is enabled!");
    }

    [HttpDelete("setup")]
    public async Task<IActionResult> Disable()
    {
        var user = await _userManager.GetUserAsync(User);

        await _userManager.SetTwoFactorEnabledAsync(user, enabled: false);

        return Ok("Two-Factor authentication is disabled!");
    }
}
