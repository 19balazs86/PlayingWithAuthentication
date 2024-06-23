using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
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

            services.AddIdentityApiEndpoints<MyIdentityUser>(configureIdentityOptions)
                .AddEntityFrameworkStores<MyDataContext>();

            services.ConfigureApplicationCookie(configureApplicationCookie);
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();

            app.MapGroup("/auth").MapIdentityApi<MyIdentityUser>();

            app.MapGet("/", (ClaimsPrincipal user) => user.Claims.ToDictionary(k => k.Type, v => v.Value)).RequireAuthorization();

            app.MapGet("/auth/fake-login", fakeLoing);
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

    private static SignInHttpResult fakeLoing()
    {
        Claim[] claims = [new Claim("id", "12345"), new Claim("name", "fake-login-name")];

        var claimsIdentity  = new ClaimsIdentity(claims, IdentityConstants.BearerScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // This will call the BearerTokenHandler.HandleSignInAsync, which generates the token response
        // Note: EntityFramework is not involved in this process, because we are not calling the login endpoint added with MapIdentityApi
        return TypedResults.SignIn(claimsPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
    }
}
