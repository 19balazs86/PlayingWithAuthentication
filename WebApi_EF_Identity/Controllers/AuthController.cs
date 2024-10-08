﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi_EF_Identity.Data;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WebApi_EF_Identity.Controllers;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password);
public record LoginRequest(string Email, string Password);

[ApiController]
[Route("[controller]")]
public sealed class AuthController : ControllerBase
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

        string emailConfirmation = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // Minimal API: You can inject the LinkGenerator and use the GetUriByName method to generate an URL for the following endpoint
        // app.MapGet("/Auth/ConfirmEmail", ([FromRoute] string email, [FromQuery] string token) => { }).WithName("ConfirmEmail")
        // emailConfirmation = linkGenerator.GetUriByName(httpContext, "ConfirmEmail", new { token = emailConfirmation, email = request.Email });
        emailConfirmation = Url.Action(nameof(ConfirmEmail), "Auth", new { token = emailConfirmation, email = request.Email }, Request.Scheme);

        return Ok(new { emailConfirmation });
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
        // var user = await _userManager.FindByEmailAsync(request.Email);
        // bool isLockedOut = await _userManager.IsLockedOutAsync(user);
        // bool isValidPassword = await _userManager.CheckPasswordAsync(user, "password");
        // SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        SignInResult result = await _signInManager.PasswordSignInAsync(userName: request.Email, request.Password, isPersistent: true, lockoutOnFailure: false);

        if (result.Succeeded)
            return Ok("You are signed in");

        string emailCode = string.Empty;

        if (result is { RequiresTwoFactor: true, IsNotAllowed: false, IsLockedOut : false })
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            string code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

            emailCode = $"You got an email about the Code: {code}";
        }

        var problemResponse = new
        {
            Title  = "Failed to login",
            Reason = result.ToString(),
            Detail = emailCode
        };

        return BadRequest(problemResponse);
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
