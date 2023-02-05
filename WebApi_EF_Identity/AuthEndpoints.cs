using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using WebApi_EF_Identity.Data;

namespace WebApi_EF_Identity;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password);
public record LoginRequest(string Email, string Password);

public static class AuthEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("Auth");

        group.MapPost("/Register",            Register);
        group.MapGet("/ConfirmEmail/{email}", ConfirmEmail);
        group.MapPost("/Login",               Login);
        group.MapGet("/Logout",               Logout).RequireAuthorization();
        group.MapGet("/Claims",               GetClaims).RequireAuthorization();
    }

    public static async Task<IResult> Register(
        RegisterRequest request,
        UserManager<MyIdentityUser> userManager)
    {
        var user = new MyIdentityUser
        {
            FirstName = request.FirstName,
            LastName  = request.LastName,
            UserName  = request.Email,
            Email     = request.Email
        };

        IdentityResult result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return getValidationProblem(result);

        // await signInManager.PasswordSignInAsync(user, request.Password, isPersistent: true, lockoutOnFailure: false);

        string emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

        emailConfirmationToken = UrlEncoder.Default.Encode(emailConfirmationToken);

        string confirmEmailUrl = $"http://localhost:5019/Auth/{nameof(ConfirmEmail)}/{request.Email}?token={emailConfirmationToken}";

        return TypedResults.Ok(new { confirmEmailUrl });
    }

    public static async Task<IResult> ConfirmEmail(
        string email,
        string token,
        UserManager<MyIdentityUser> userManager,
        SignInManager<MyIdentityUser> signInManager)
    {
        const string errorMessage = "Invalid email or token.";

        var user = await userManager.FindByEmailAsync(email);

        if (user is null || user.EmailConfirmed) // Email confirmed, skip the process.
            return TypedResults.BadRequest(errorMessage);

        IdentityResult result = await userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return getValidationProblem(result);

        await signInManager.SignInAsync(user, isPersistent: true);

        return TypedResults.Ok("You are signed in");
    }

    public static async Task<IResult> Login(
        LoginRequest request,
        UserManager<MyIdentityUser> userManager,
        SignInManager<MyIdentityUser> signInManager)
    {
        //var user = await userManager.FindByEmailAsync(request.Email);
        //SignInResult result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        SignInResult result = await signInManager.PasswordSignInAsync(userName: request.Email, request.Password, isPersistent: true, lockoutOnFailure: false);

        if (result.Succeeded)
            return TypedResults.Ok("You are signed in");

        return TypedResults.Problem(title: "Failed to login", statusCode: StatusCodes.Status400BadRequest);
    }

    public static async Task Logout(SignInManager<MyIdentityUser> signInManager)
    {
        await signInManager.SignOutAsync();
    }

    public static IResult GetClaims(HttpRequest request)
    {
        var claims = request.HttpContext.User.Claims;

        var response = claims.Select(c => new { c.Type, c.Value });

        return TypedResults.Ok(response);
    }

    private static IResult getValidationProblem(IdentityResult result)
    {
        Dictionary<string, string[]> errors = result.Errors.ToDictionary(x => x.Code, x => new string[] { x.Description });

        return TypedResults.ValidationProblem(errors, title: "Something went wrong");
    }
}
