using Auth0.AspNetCore.Authentication;
using BlazorWASM.BFF.Auth0.OpenIDConnect.Shared.Defaults;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Mime;
using System.Text.Json;

namespace BlazorWASM.BFF.Auth0.OpenIDConnect.Server;

public static class Program
{
    private static readonly PathString _apiPrefix = new PathString("/api");

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;
        IConfiguration configuration  = builder.Configuration;

        // Add services to the container
        {
            // Use IDbContextFactory with AddDbContextFactory instead of AddDbContext.
            // Dependency Injection scopes in Blazor: https://www.thinktecture.com/en/blazor/dependency-injection-scopes-in-blazor


            services.addAntiforgeryServices();

            services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            services.AddRazorPages();

            services.addAuth0Authentication(configuration);
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseHttpsRedirection();

            // Package: https://github.com/andrewlock/NetEscapades.AspNetCore.SecurityHeaders
            // https://github.com/damienbod/Blazor.BFF.OpenIDConnect.Template/blob/main/BlazorBffOpenIdConnect/Server/SecurityHeadersDefinitions.cs
            app.UseSecurityHeaders();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.useHeaderRequestedWith();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();
            app.mapApiNotFound();
            app.MapFallbackToPage("/_Host");
        }

        app.Run();
    }

    private static void addAuth0Authentication(this IServiceCollection services, IConfiguration configuration)
    {
        // This extension method can be used for simplicity
        // services.AddAuth0WebAppAuthentication(options => { });

        // Adding Authentication and Cookie manually provides more control
        services.AddAuthentication(options =>
        {
            options.DefaultScheme          = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            // Unauthorized calls will be challenged and redirected to Auth0, which is undesirable
            // Redirection causing exception in AuthorizedHandler.
            // If you ConfigurePrimaryHttpMessageHandler with AllowAutoRedirect = false, then StatusCode return 0
            // options.DefaultChallengeScheme = Auth0Constants.AuthenticationScheme;
        })
        .AddCookie(options => options.Cookie.Name = "AuthCookie")
        .AddAuth0WebAppAuthentication(options => configureAuth0Options(options, configuration));
    }

    private static void configureAuth0Options(Auth0WebAppOptions options, IConfiguration configuration)
    {
        // Bind the following values: Domain, ClientId, SkipCookieMiddleware, Scope
        configuration.GetSection("Authentication:Auth0").Bind(options);

        //options.ClientSecret = configuration.GetValue<string>("Authentication:Auth0:ClientSecret");
        //options.ResponseType = OpenIdConnectResponseType.Code;

        options.OpenIdConnectEvents = new OpenIdConnectEvents { OnTicketReceived = onTicketReceived };
    }

    private static void mapApiNotFound(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.Map("/api/{**segment}", async context =>
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

    private static void useHeaderRequestedWith(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Use((httpContext, func) =>
        {
            if (httpContext.Request.Path.StartsWithSegments(_apiPrefix))
                httpContext.Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.XRequestedWith] = "XMLHttpRequest";

            return func();
        });
    }

    private static void addAntiforgeryServices(this IServiceCollection services)
    {
        services.AddAntiforgery(options =>
        {
            options.HeaderName          = AntiforgeryDefaults.HeaderName;
            options.Cookie.Name         = AntiforgeryDefaults.CookieName;
            options.Cookie.SameSite     = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
    }

    private static Task onTicketReceived(TicketReceivedContext context)
    {
        // This event can be used
        // 1) Replace the context.Principal by creating a new one to sign-in
        // 2) Based on a claim (NameIdentifier), the user can be identified, and you can create or retrieve the user from the database

        // context.HttpContext.RequestServices
        // context.Principal = new ClaimsPrincipal();

        return Task.CompletedTask;
    }
}
