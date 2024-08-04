using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace ApiCookieAuth.Essentials;

public static class Endpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder routeGroup = endpoints.MapGroup("/auth");

        routeGroup.MapGet("/user", handleUser);
        routeGroup.MapGet("/login", handleLogin);
        routeGroup.MapGet("/logout", handleLogout).RequireAuthorization();
        routeGroup.MapGet("/black-list", handleBlackList).RequireAuthorization();
    }

    private static Ok<UserModel> handleUser(ClaimsPrincipal claimsPrincipal)
    {
        return TypedResults.Ok(UserModel.CreateFromPrincipal(claimsPrincipal));
    }

    private static SignInHttpResult handleLogin()
    {
        var authProperties = new AuthenticationProperties
        {
            //IsPersistent = true, // It keeps the cookie on the client side, otherwise it is a session cookie
            //ExpiresUtc = DateTime.UtcNow.AddDays(15), // You can use ExpiresUtc or globally set ExpireTimeSpan when you call AddCookie
            RedirectUri = "/auth/user"
        };

        // You can add secrets to the authProperties.StoreTokens(tokens), which are stored in the client's auth-cookie
        // You can retrieve them via HttpContext.GetToken

        var claims = new UserModel(1, "DummyUser", ["UserRole"]).ToClaims();

        var claimsIdentity = new ClaimsIdentity(claims, Program.DefaultAuthScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return TypedResults.SignIn(claimsPrincipal, authProperties, Program.DefaultAuthScheme);
    }

    private static SignOutHttpResult handleLogout()
    {
        var authProperties = new AuthenticationProperties { RedirectUri = "/auth/user" };

        return TypedResults.SignOut(authProperties, [Program.DefaultAuthScheme]);
    }

    private static Ok<string> handleBlackList(ClaimsPrincipal claimsPrincipal)
    {
        UserModel user = UserModel.CreateFromPrincipal(claimsPrincipal);

        Program.CookieBlackList.Add(user.SessionId);

        return TypedResults.Ok("You added yourself to the blacklist!");
    }
}
