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
            services.AddControllers();

            services.AddProblemDetails();

            services.AddDbContext<MyDataContext>(options => options.UseInMemoryDatabase("data"));

            services
                .AddIdentity<MyIdentityUser, IdentityRole>(identityOptions =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        identityOptions.SignIn.RequireConfirmedEmail = true;

                        identityOptions.User.RequireUniqueEmail = true;

                        identityOptions.Password.RequireNonAlphanumeric = false;
                        identityOptions.Password.RequireDigit           = false;
                    }
                })
                .AddEntityFrameworkStores<MyDataContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(optinos =>
            {
                optinos.ExpireTimeSpan = TimeSpan.FromSeconds(30);

                // Since there is no front-end, we need to change the default behavior.
                // Do not redirect to "/Account/Login"

                optinos.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                optinos.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

            services.AddAuthorization();
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        {
            app.UseExceptionHandler();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }

        app.Run();
    }
}