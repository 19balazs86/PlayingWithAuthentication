using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApi_EF_Identity_BearerNET8.Data;

namespace WebApi_EF_Identity_BearerNET8;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;

        // Add services to the container
        {
            services.AddAuthorization();

            services.AddSwaggerGen();
            services.AddEndpointsApiExplorer();

            services.AddDbContext<MyDataContext>(options => options.UseInMemoryDatabase("data"));

            // This line applies the AddAuthentication -> AddBearerToken and AddIdentityCookies methods
            services.AddIdentityApiEndpoints<MyIdentityUser>(configureIdentityOptions)
                .AddEntityFrameworkStores<MyDataContext>();

            services.ConfigureApplicationCookie(configureApplicationCookie);
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            //app.UseAuthentication();
            app.UseAuthorization();

            app.MapGroup("/auth").MapIdentityApi<MyIdentityUser>();

            app.MapGet("/", (ClaimsPrincipal user) => user.Claims.ToDictionary(k => k.Type, v => v.Value)).RequireAuthorization();

            app.MapGet("/auth/fake-login", handleFakeLoing);
        }

        app.Run();
    }

    private static void configureIdentityOptions(IdentityOptions options)
    {
        options.SignIn.RequireConfirmedEmail = false;

        options.User.RequireUniqueEmail = true;

        options.Password.RequireDigit           = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase       = false;
    }

    private static void configureApplicationCookie(CookieAuthenticationOptions optinos)
    {
        optinos.ExpireTimeSpan = TimeSpan.FromDays(1);

        optinos.Cookie.Name = "auth-cookie";
    }

    private static SignInHttpResult handleFakeLoing([FromQuery] bool? useCookies)
    {
        string authShame   = IdentityConstants.BearerScheme;
        var authProperties = new AuthenticationProperties();

        if (useCookies is true)
        {
            authShame = IdentityConstants.ApplicationScheme;

            authProperties.IsPersistent = true;
        }

        Claim[] claims = [new Claim("id", "12345"), new Claim("name", "fake-login-name")];

        var claimsIdentity  = new ClaimsIdentity(claims, authShame);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // BearerScheme: This will call the BearerTokenHandler.HandleSignInAsync, which generates the token response
        // ApplicationScheme: Call the CookieAuthenticationHandler.HandleSignInAsync
        // Note: EntityFramework is not involved in this process, because we are not calling the login endpoint added with MapIdentityApi
        return TypedResults.SignIn(claimsPrincipal, authProperties, authShame);
    }
}
