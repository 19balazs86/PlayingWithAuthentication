using Microsoft.AspNetCore.Identity;
using WebApi_EF_Identity.Data;

namespace WebApi_EF_Identity;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password);
public record LoginRequest(string Email, string Password);

public static class Endpoints
{
    public static async Task<IResult> Register(
        RegisterRequest request,
        UserManager<MyIdentityUser> userManager,
        SignInManager<MyIdentityUser> signInManager)
    {
        var user = new MyIdentityUser
        {
            FirstName = request.FirstName,
            LastName  = request.LastName,
            UserName  = request.Email,
            Email     = request.Email
        };

        IdentityResult result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await signInManager.PasswordSignInAsync(user, request.Password, isPersistent: true, lockoutOnFailure: false);

            return TypedResults.Ok();
        }

        Dictionary<string, string[]> errors = result.Errors.ToDictionary(x => x.Code, x => new string[] { x.Description });

        return TypedResults.ValidationProblem(errors, title: "Registration falied");
    }

    public static async Task<IResult> Login(LoginRequest request, SignInManager<MyIdentityUser> signInManager)
    {
        //var user = await userManager.FindByEmailAsync(request.Email);
        //SignInResult result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        SignInResult result = await signInManager.PasswordSignInAsync(userName: request.Email, request.Password, isPersistent: true, lockoutOnFailure: false);

        if (result.Succeeded)
            return TypedResults.Ok();

        return TypedResults.Problem(title: "Failed to login", statusCode: StatusCodes.Status400BadRequest);
    }

    public static async Task Logout(SignInManager<MyIdentityUser> signInManager)
    {
        await signInManager.SignOutAsync();
    }
}
