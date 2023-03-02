using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using WebApi_EF_Identity.Data;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WebApi_EF_Identity.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public sealed class TwoFactorAuthController : ControllerBase
{
    private const string _authenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
    private const string _qrServerUrlFormat      = "https://api.qrserver.com/v1/create-qr-code/?size=200x200&data={0}";

    private readonly UserManager<MyIdentityUser> _userManager;
    private readonly SignInManager<MyIdentityUser> _signInManager;
    private readonly UrlEncoder _urlEncoder;

    private readonly string _tokenProvider;

    public TwoFactorAuthController(UserManager<MyIdentityUser> userManager, SignInManager<MyIdentityUser> signInManager, UrlEncoder urlEncoder)
    {
        _userManager   = userManager;
        _signInManager = signInManager;
        _urlEncoder    = urlEncoder;

        _tokenProvider = _userManager.Options.Tokens.AuthenticatorTokenProvider;
    }

    [AllowAnonymous]
    [HttpPost("Login/{code}")]
    public async Task<IActionResult> Login(string code)
    {
        // In order to make 2FA work, you need to log in with username and password to create the 'Identity.TwoFactorUserId' cookie.
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        if (user is null)
            return Unauthorized("First you need to login with user name and password");

        //bool validVerification = await _userManager.VerifyTwoFactorTokenAsync(user, _tokenProvider, request.Code);

        SignInResult result = await _signInManager.TwoFactorAuthenticatorSignInAsync(code, isPersistent: true, rememberClient: true);

        if (result.Succeeded)
            return Ok("You are signed in");

        return Problem(title: "Failed to login", detail: result.ToString(), statusCode: StatusCodes.Status400BadRequest);
    }

    [AllowAnonymous]
    [HttpPost("LoginWithRecoveryCode/{recoveryCode}")]
    public async Task<IActionResult> LoginWithRecoveryCode(string recoveryCode)
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

    [HttpGet("setup")]
    public async Task<IActionResult> GetSetup()
    {
        //string email = User.Identity.Name;

        var user = await _userManager.GetUserAsync(User);

        bool isTwoFactorAuthEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

        string authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);

        if (string.IsNullOrWhiteSpace(authenticatorKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        string qrCodeUrl = generateQRCode(user.Email, authenticatorKey);
        qrCodeUrl        = get_QR_ServerUrl(qrCodeUrl);

        return Ok(new { TwoFactorAuthEnabled = isTwoFactorAuthEnabled, AuthenticatorKey = authenticatorKey, QrCode = qrCodeUrl });
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

    [HttpGet("RecoveryCodes")]
    public async Task<IActionResult> GetRecoveryCodes()
    {
        var user = await _userManager.GetUserAsync(User);

        IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 5);

        return Ok(new { Message = "You can use 1 of the following codes to login and ignore Two-Factor authentication.", recoveryCodes });
    }

    private string get_QR_ServerUrl(string otpAuth)
    {
        return string.Format(_qrServerUrlFormat, _urlEncoder.Encode(otpAuth));
    }

    private static string generateQRCode(string email, string authenticatorKey)
    {
        const string issuer = "2Factor-Auth";

        return string.Format(_authenticatorUriFormat, issuer, email, authenticatorKey);
    }
}
