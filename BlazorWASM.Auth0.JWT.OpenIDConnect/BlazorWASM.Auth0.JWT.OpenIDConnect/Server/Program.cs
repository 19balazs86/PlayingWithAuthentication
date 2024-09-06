using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mime;
using System.Text.Json;

namespace BlazorWASM.Auth0.JWT.OpenIDConnect;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;
        IConfiguration configuration  = builder.Configuration;

        // Add services to the container
        {
            services.AddControllers();

            services.addJwtAuthentication(configuration);
        }

        WebApplication app = builder.Build();

        app.configureRequestPipeline();

        app.Run();
    }

    private static void addJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        OidcConfig? oidcConfig = configuration
           .GetSection(nameof(OidcConfig))
           .Get<OidcConfig>()
           ?? throw new NullReferenceException("OpenIdcConfig was not found in appsettings.json");

        services.AddSingleton(oidcConfig);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority       = oidcConfig.Authority;
            options.Audience        = oidcConfig.Audience;
            options.MetadataAddress = oidcConfig.MetadataUrl; // If you do not set this value, the default will be '<Authority>/.well-known/openid-configuration,' which is a correct value

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer      = true,
                ValidIssuers        = oidcConfig.ValidIssuers(),
                ValidateAudience    = true,
                ValidateLifetime    = true,
                ValidateTokenReplay = true
            };
        });
    }

    private static void configureRequestPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler();
        }

        app.UseHsts();
        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.mapApiNotFound();
        app.MapFallbackToFile("index.html");
    }

    /// <summary>
    /// Avoid returning index.html for not found API calls in Blazor WASM
    /// https://peterlesliemorris.com/avoid-returning-index-html-for-api-calls
    /// </summary>
    private static void mapApiNotFound(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapFallback("/api/{*path}", async context =>
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode  = StatusCodes.Status404NotFound;

            var problemDetails = new
            {
                Title = "The requested endpoint is not found.",
                Status = StatusCodes.Status404NotFound,
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, problemDetails);
        });
    }
}
