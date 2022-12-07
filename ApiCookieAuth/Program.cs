using Microsoft.AspNetCore.Authentication.Cookies;

namespace ApiCookieAuth
{
    public class Program
    {
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
                });


            services.AddAuthorization();

            services.AddSingleton<ApiJwtClientAuthHandler>();

            services.AddHttpContextAccessor();

            services.AddHttpClient<IApiJwtClient, ApiJwtClient>(configureClient =>
            {
                configureClient.BaseAddress = new Uri("https://localhost:5000");
            }).AddHttpMessageHandler<ApiJwtClientAuthHandler>();
        }
    }
}