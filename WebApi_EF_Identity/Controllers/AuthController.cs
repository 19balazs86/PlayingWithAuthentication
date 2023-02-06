using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using WebApi_EF_Identity.Data;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WebApi_EF_Identity.Controllers;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password);
public record LoginRequest(string Email, string Password);

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<MyIdentityUser> _userManager;
    private readonly SignInManager<MyIdentityUser> _signInManager;

    public AuthController(UserManager<MyIdentityUser> userManager, SignInManager<MyIdentityUser> signInManager)
    {
        _userManager   = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new MyIdentityUser
        {
            FirstName = request.FirstName,
            LastName  = request.LastName,
            UserName  = request.Email,
            Email     = request.Email
        };

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return getValidationProblem(result);

        // await signInManager.PasswordSignInAsync(user, request.Password, isPersistent: true, lockoutOnFailure: false);

        string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        emailConfirmationToken = UrlEncoder.Default.Encode(emailConfirmationToken);

        string confirmEmailUrl = $"http://localhost:5019/Auth/{nameof(ConfirmEmail)}/{request.Email}?token={emailConfirmationToken}";

        return Ok(new { confirmEmailUrl });
    }

    [HttpGet("ConfirmEmail/{email}")]
    public async Task<IActionResult> ConfirmEmail([FromRoute] string email, [FromQuery] string token)
    {
        const string errorMessage = "Invalid email or token.";

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || user.EmailConfirmed) // Email confirmed, skip the process.
            return BadRequest(errorMessage);

        IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return getValidationProblem(result);

        await _signInManager.SignInAsync(user, isPersistent: true);

        return Ok("You are signed in");
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        //var user = await _userManager.FindByEmailAsync(request.Email);
        //SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        SignInResult result = await _signInManager.PasswordSignInAsync(userName: request.Email, request.Password, isPersistent: true, lockoutOnFailure: false);

        if (result.Succeeded)
            return Ok("You are signed in");

        return Problem(title: "Failed to login", detail: result.ToString(), statusCode: StatusCodes.Status400BadRequest);
    }

    [Authorize]
    [HttpGet("Logout")]
    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }

    [Authorize]
    [HttpGet("Claims")]
    public IActionResult Claims()
    {
        var claims = HttpContext.User.Claims;

        var response = claims.Select(c => new { c.Type, c.Value });

        return Ok(response);
    }

    private IActionResult getValidationProblem(IdentityResult result)
    {
        Dictionary<string, string[]> errors = result.Errors.ToDictionary(x => x.Code, x => new string[] { x.Description });

        var problem = new ValidationProblemDetails(errors) { Title = "Something went wrong" };

        return ValidationProblem(problem);
    }
}
