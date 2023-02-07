using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ApiCookieAuth;

public class Program
{
    public const string SessionClaimName = "SessionId";

    public static List<string> CookieBlackList { get; } = new List<string>();

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        configureServices(builder.Services);

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void configureServices(IServiceCollection services)
    {
        services.AddControllers();

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath   = "/login.html";
                options.Cookie.Name = "auth-cookie";
                //options.ExpireTimeSpan = TimeSpan.FromDays(15); // During HttpContext.SignIn the AuthenticationProperties needs to be set IsPersistent = true

                options.Events.OnValidatePrincipal = onValidatePrincipal;
            });


        services.AddAuthorization();

        services.AddTransient<ApiJwtClientAuthHandler>();

        services.AddHttpContextAccessor();

        services.AddHttpClient<IApiJwtClient, ApiJwtClient>(configureClient =>
        {
            configureClient.BaseAddress = new Uri("https://localhost:5000");
        }).AddHttpMessageHandler<ApiJwtClientAuthHandler>();
    }

    private static Task onValidatePrincipal(CookieValidatePrincipalContext ctx)
    {
        // IServiceProvider serviceProvider = ctx.HttpContext.RequestServices;

        // To invalidate a cookie, it can can be rejected based on the session id.

        if (CookieBlackList.Contains(ctx.Principal.FindFirstValue(SessionClaimName)))
            ctx.RejectPrincipal();

        return Task.CompletedTask;
    }
}