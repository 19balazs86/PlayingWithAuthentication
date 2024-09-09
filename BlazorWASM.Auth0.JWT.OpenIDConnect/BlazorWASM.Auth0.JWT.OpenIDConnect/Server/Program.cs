using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net.Mime;
using System.Text.Json;

namespace BlazorWASM.Auth0.JWT.OpenIDConnect.Server;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;
        IConfiguration configuration  = builder.Configuration;

        OidcConfig? oidcConfig = configuration
           .GetSection(nameof(OidcConfig))
           .Get<OidcConfig>()
           ?? throw new NullReferenceException("OidcConfig was not found in appsettings.json");

        // Add services to the container
        {
            services.AddSingleton(oidcConfig);

            services.AddControllers();

            services.addJwtAuthentication(oidcConfig);

            services.addSwaggerWithOAuth(oidcConfig);
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();

                // https://localhost:7209/swagger/index.html
                app.UseSwagger();
                app.UseSwaggerUI(options => options.OAuthClientId(oidcConfig.ClientId));
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

        app.Run();
    }

    private static void addJwtAuthentication(this IServiceCollection services, OidcConfig oidcConfig)
    {
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

            options.MapInboundClaims = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer   = true,
                ValidateAudience = true,
                ValidateLifetime = true,

                ValidIssuers = oidcConfig.ValidIssuers(),

                NameClaimType = "sub",
                RoleClaimType = "role"
            };
        });
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

    // Swagger is not required, but it serves as an example for integrating with Auth0
    private static void addSwaggerWithOAuth(this IServiceCollection services, OidcConfig oidcConfig)
    {
        var authorizationUrlBuilder = new UriBuilder(oidcConfig.Authority)
        {
            Path  = "authorize",
            Query = $"audience={oidcConfig.Audience}"
        };

        const string schemeName = "OAuth2";

        services.AddSwaggerGen(options =>
        {
            var scheme = new OpenApiSecurityScheme
            {
                Type  = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = authorizationUrlBuilder.Uri,
                        Scopes           = new Dictionary<string, string>
                        {
                            ["openid"]  = "openid",
                            ["profile"] = "profile",
                            ["email"]   = "email"
                        }
                    }
                }
            };

            options.AddSecurityDefinition(schemeName, scheme);

            var key = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = schemeName
                },
                In     = ParameterLocation.Header,
                Name   = "Bearer",
                Scheme = "Bearer"
            };

            var requirement = new OpenApiSecurityRequirement
            {
                { key, [] }
            };

            options.AddSecurityRequirement(requirement);
        });
    }
}
