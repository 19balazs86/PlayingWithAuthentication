using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using WebApi_EF_Identity.Data;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WebApi_EF_Identity.Controllers;

[ApiController]
[Route("[controller]")]
public class TwoFactorAuthController : ControllerBase
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

    [HttpPost("Login/{code}")]
    public async Task<IActionResult> Login(string code)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        if (user is null)
            return Unauthorized("Invalid authentication");

        //bool validVerification = await _userManager.VerifyTwoFactorTokenAsync(user, _tokenProvider, request.Code);

        SignInResult result = await _signInManager.TwoFactorAuthenticatorSignInAsync(code, isPersistent: true, rememberClient: true);

        if (result.Succeeded)
            return Ok("You are signed in");

        return Problem(title: "Failed to login", detail: result.ToString(), statusCode: StatusCodes.Status400BadRequest);
    }

    [HttpGet("setup")]
    [Authorize]
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
    [Authorize]
    public async Task<IActionResult> Enable(string code)
    {
        var user = await _userManager.GetUserAsync(User);

        bool isValidCode = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);

        if (!isValidCode)
            return BadRequest("Invalid code");

        await _userManager.SetTwoFactorEnabledAsync(user, enabled: true);

        return Ok("Two-Factor authentication is enabled!");
    }

    [HttpDelete("setup")]
    [Authorize]
    public async Task<IActionResult> Disable()
    {
        var user = await _userManager.GetUserAsync(User);

        await _userManager.SetTwoFactorEnabledAsync(user, enabled: false);

        return Ok("Two-Factor authentication is disabled!");
    }

    private string get_QR_ServerUrl(string otpAuth)
    {
        return string.Format(_qrServerUrlFormat, _urlEncoder.Encode(otpAuth));
    }

    private string generateQRCode(string email, string authenticatorKey)
    {
        return string.Format(
            _authenticatorUriFormat,
            _urlEncoder.Encode("2Factor-Auth"),
            _urlEncoder.Encode(email),
            authenticatorKey);
    }
}
