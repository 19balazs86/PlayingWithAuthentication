using ApiKeyAuth.Solutions;
using Microsoft.OpenApi.Models;

namespace ApiKeyAuth;

public sealed class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;

        // Add services to the container
        {
            services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();

            services.AddSingleton<ApiKeyAuthFilter>();

            // TODO: Solution 2/a - Add Authorization filter for all endpoints of the Controller
            //services.AddControllers(options => options.Filters.Add<ApiKeyAuthFilter>());

            services.AddControllers();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerWithApiKeyAuth();

            // TODO: Solution 4 - Add custom authentication handler (I think the most complex compare with the others)
            // Use it as usual with [Authorize] attribute
            //services.ApiKeyAuthentication();
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            // TODO: Solution 1 - Use a custom middleware to check the API Key
            //app.UseApiKeyAuthMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // TODO: Solution 3 - Add endpoint filter
            app.MapGet("WeatherForecastMini", () => WeatherForecast.GetRandomForecasts())
                .AddEndpointFilter<ApiKeyAuthEndpointFilter>();
        }

        app.Run();
    }
}

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithApiKeyAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var scheme = new OpenApiSecurityScheme
            {
                Description = "Provide an API Key to access the API",
                Type        = SecuritySchemeType.ApiKey,
                Name        = ApiKeyValidator.ApiKeyHeaderName,
                In          = ParameterLocation.Header,
                Scheme      = "ApiKeyScheme"
            };

            options.AddSecurityDefinition(nameof(SecuritySchemeType.ApiKey), scheme);

            var key = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = nameof(SecuritySchemeType.ApiKey)
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