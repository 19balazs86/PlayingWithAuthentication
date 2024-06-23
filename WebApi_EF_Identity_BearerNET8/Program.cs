using Microsoft.AspNetCore.Authentication.Cookies;
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
}
