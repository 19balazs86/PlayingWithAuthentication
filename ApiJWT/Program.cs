using Microsoft.AspNetCore.Authorization;

namespace ApiJWT
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
            configureApplication(app);

            app.Run();
        }

        private static void configureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddJwtAuthentication();

            services.AddAuthorization(options =>
            {
                // Add policy for Admin role.
                options.AddPolicy("Admin", new AuthorizationPolicyBuilder().RequireRole("Admin").Build());

                // -> https://andrewlock.net/setting-global-authorization-policies-using-the-defaultpolicy-and-the-fallbackpolicy-in-aspnet-core-3
                // -> https://docs.microsoft.com/en-ie/aspnet/core/migration/22-to-30?view=aspnetcore-3.0&tabs=visual-studio#authorization
                // FallbackPolicy is initially configured to allow requests without authorization.
                // Override it in order to require authentication on all endpoints except when [AllowAnonymous].
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        private static void configureApplication(WebApplication app)
        {
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}