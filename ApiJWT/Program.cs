using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace ApiJWT;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;

        // Add services to the container
        {
            services.AddControllers();

            services.addSwaggerWithJwtAuth();

            services.AddJwtAuthentication();

            services.AddAuthorization(configureAuthorizationOptions);
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }

        app.Run();
    }

    private static void configureAuthorizationOptions(AuthorizationOptions options)
    {
        // "Claims transformation for flexible Authorization": See the link in the Readme
        // Add policy for Admin role.
        options.AddPolicy("AdminPolicy", new AuthorizationPolicyBuilder().RequireRole("AdminRole").Build());

        // -> https://andrewlock.net/setting-global-authorization-policies-using-the-defaultpolicy-and-the-fallbackpolicy-in-aspnet-core-3
        // -> https://docs.microsoft.com/en-ie/aspnet/core/migration/22-to-30?view=aspnetcore-3.0&tabs=visual-studio#authorization
        // FallbackPolicy is initially configured to allow requests without authorization.
        // Override it in order to require authentication on all endpoints except when [AllowAnonymous].
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    }

    private static IServiceCollection addSwaggerWithJwtAuth(this IServiceCollection services)
    {
        const string schemeName = "Bearer";

        services.AddSwaggerGen(options =>
        {
            var scheme = new OpenApiSecurityScheme
            {
                Description  = "Provide a JWT in the following format: 'Bearer YourToken'",
                Type         = SecuritySchemeType.ApiKey,
                Name         = "Authorization",
                In           = ParameterLocation.Header,
                Scheme       = schemeName,
                BearerFormat = "JWT"
            };

            options.AddSecurityDefinition(schemeName, scheme);

            var key = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = schemeName
                },
            };

            var requirement = new OpenApiSecurityRequirement
            {
                { key, new List<string>() }
            };

             options.AddSecurityRequirement(requirement);
        });

        return services;
    }
}