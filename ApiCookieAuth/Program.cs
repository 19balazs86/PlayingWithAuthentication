using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ApiCookieAuth;

public static class Program
{
    public const string DefaultAuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    public static HashSet<string> CookieBlackList { get; } = []; // This should stored in database or redis

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;

        // Add services to the container
        {
            services.AddAuthentication(DefaultAuthScheme)
                .AddCookie(DefaultAuthScheme, configureCookieAuthOptions);

            services.AddAuthorization();

            services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapAuthEndpoints();

            app.MapGet("/", () => TypedResults.Redirect("/home.html"));
        }

        app.Run();
    }

    private static void configureCookieAuthOptions(CookieAuthenticationOptions options)
    {
        options.LoginPath   = "/home.html";
        options.Cookie.Name = "auth-cookie";
        //options.ExpireTimeSpan = TimeSpan.FromDays(15); // During HttpContext.SignIn the AuthenticationProperties needs to be set IsPersistent = true

        options.Events.OnValidatePrincipal = onValidatePrincipal;

        // When you are unauthorized, the server redirects you to LoginPath. However, you can prevent this by adding the following:
        //options.Events.OnRedirectToLogin        = context => preventRedirect(context, 401);
        //options.Events.OnRedirectToAccessDenied = context => preventRedirect(context, 403);
    }

    private static Task onValidatePrincipal(CookieValidatePrincipalContext ctx)
    {
        // IServiceProvider serviceProvider = ctx.HttpContext.RequestServices;

        if (CookieBlackList.Contains(ctx.Principal!.FindFirstValue(UserModel.SessionClaimName)!))
            ctx.RejectPrincipal(); // To invalidate a cookie, it can can be rejected based on the session id.

        return Task.CompletedTask;
    }

    private static Task preventRedirect(RedirectContext<CookieAuthenticationOptions> context, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        return Task.CompletedTask;
    }
}