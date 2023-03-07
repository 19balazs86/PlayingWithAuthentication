using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ApiCookieAuth;

public static class Program
{
    public const string SessionClaimName = "SessionId";

    public static List<string> CookieBlackList { get; } = new List<string>();

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;

        // Add services to the container
        {
            services.AddControllers();

            services
                .AddAuthentication()
                .AddCookie(configureCookieAuthenticationOptions);

            services.AddAuthorization();

            services.AddTransient<ApiJwtClientAuthHandler>();

            services.AddHttpContextAccessor();

            services
                .AddHttpClient<IApiJwtClient, ApiJwtClient>(client => client.BaseAddress = new Uri("https://localhost:5000"))
                .AddHttpMessageHandler<ApiJwtClientAuthHandler>();
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }

        app.Run();
    }

    private static void configureCookieAuthenticationOptions(CookieAuthenticationOptions options)
    {
        options.LoginPath   = "/login.html";
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

        // To invalidate a cookie, it can can be rejected based on the session id.

        if (CookieBlackList.Contains(ctx.Principal.FindFirstValue(SessionClaimName)))
            ctx.RejectPrincipal();

        return Task.CompletedTask;
    }

    private static Task preventRedirect(RedirectContext<CookieAuthenticationOptions> context, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        return Task.CompletedTask;
    }
}