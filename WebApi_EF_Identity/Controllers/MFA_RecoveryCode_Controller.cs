using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi_EF_Identity.Data;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WebApi_EF_Identity.Controllers;

[Authorize]
[ApiController]
[Route("2FA-RecoveryCode")]
public sealed class MFA_RecoveryCode_Controller : ControllerBase
{
    private readonly UserManager<MyIdentityUser> _userManager;
    private readonly SignInManager<MyIdentityUser> _signInManager;

    public MFA_RecoveryCode_Controller(UserManager<MyIdentityUser> userManager, SignInManager<MyIdentityUser> signInManager)
    {
        _userManager   = userManager;
        _signInManager = signInManager;
    }

    [AllowAnonymous]
    [HttpPost("Login/{recoveryCode}")]
    public async Task<IActionResult> Login(string recoveryCode)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        if (user is null)
            return Unauthorized("First you need to login with user name and password");

        //IdentityResult result = await _userManager.RedeemTwoFactorRecoveryCodeAsync(user, recoveryCode);

        SignInResult result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        if (!result.Succeeded)
            return Problem(title: "Failed to login", detail: result.ToString(), statusCode: StatusCodes.Status400BadRequest);

        int countRecoveryCodes = await _userManager.CountRecoveryCodesAsync(user);

        return Ok($"You are signed in with recovery code. {countRecoveryCodes} number of code left.");
    }

    [HttpGet]
    public async Task<IActionResult> GetRecoveryCodes()
    {
        var user = await _userManager.GetUserAsync(User);

        IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 5);

        return Ok(new { Message = "You can use 1 of the following codes to login and ignore Two-Factor authentication.", recoveryCodes });
    }
}
