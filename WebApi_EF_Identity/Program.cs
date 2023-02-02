using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi_EF_Identity.Data;

namespace WebApi_EF_Identity;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;

        // Add services to the container.
        {
            services.AddProblemDetails();

            services.AddDbContext<MyDataContext>(options => options.UseInMemoryDatabase("data"));
            services
                .AddIdentity<MyIdentityUser, IdentityRole>(identityOptions =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        identityOptions.User.RequireUniqueEmail = true;

                        identityOptions.Password.RequireNonAlphanumeric = false;
                        identityOptions.Password.RequireDigit           = false;
                    }
                })
                .AddEntityFrameworkStores<MyDataContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization();
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        {
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseExceptionHandler();

            app.MapPost("/register", Endpoints.Register);
            app.MapPost("/login",    Endpoints.Login);
            app.MapGet("/logout",    Endpoints.Logout).RequireAuthorization();
        }

        app.Run();
    }
}