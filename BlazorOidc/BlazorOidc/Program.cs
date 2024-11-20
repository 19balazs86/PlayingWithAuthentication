using BlazorOidc.Components;
using BlazorOidc.Endpoints;
using BlazorOidc.Miscellaneous;
using BlazorOidc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SharedLib;

namespace BlazorOidc;

public static class Program
{
    public const string AuthOidcScheme = "OidcAuth0";

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder  = WebApplication.CreateBuilder(args);
        IServiceCollection    services = builder.Services;

        // Add services to the container
        {
            OidcConfig oidcConfig = builder.addOidcConfig();

            services.AddRazorComponents()
                    .AddInteractiveServerComponents()
                    .AddInteractiveWebAssemblyComponents()
                    .AddAuthenticationStateSerialization(configure => configure.SerializeAllClaims = true);

            services.AddAuthorization();
            services.AddCascadingAuthenticationState();

            services.AddAuthentication(options =>
                    {
                        options.DefaultScheme          = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultSignInScheme    = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = AuthOidcScheme;
                    })
                    .AddCookie(options => options.Cookie.Name = "AuthCookie")
                    .AddOpenIdConnect(AuthOidcScheme, oidcOptions => configureOpenIdConnect(oidcOptions, oidcConfig));

            builder.Services.AddScoped<IWeatherForecaster, ServerWeatherForecaster>();

            // services.ConfigureCookieOidcRefresh();
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseWebAssemblyDebugging();

            app.MapStaticAssets();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAntiforgery(); // Use it after authentication, otherwise, it will cause headache

            app.MapAuthEndpoints()
               .MapWeatherForecastEndpoints();

            app.MapRazorComponents<App>()
               .AddInteractiveServerRenderMode()
               .AddInteractiveWebAssemblyRenderMode()
               .AddAdditionalAssemblies(typeof(Client.Components._Imports).Assembly);
        }

        app.Run();
    }

    private static void configureOpenIdConnect(OpenIdConnectOptions oidcOptions, OidcConfig oidcConfig)
    {
        // The OIDC handler must use a sign-in scheme capable of persisting user credentials across requests
        oidcOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        // oidcOptions.CallbackPath = "/signin-oidc";

        oidcOptions.Authority       = oidcConfig.Authority;
        oidcOptions.ClientId        = oidcConfig.ClientId;
        oidcOptions.MetadataAddress = oidcConfig.MetadataUrl;
        // oidcOptions.ClientSecret    = oidcConfig.ClientSecret; // The `client_secret` is not required in Authorization Code Flow

        oidcOptions.Scope.Add(OpenIdConnectScope.Email); // The "openid" and "profile" scopes are required for the OIDC handler and included by default

        oidcOptions.AdditionalAuthorizationParameters.Add("audience", oidcConfig.Audience);

        oidcOptions.ResponseType = OpenIdConnectResponseType.Code; // Authorization Code Flow

        oidcOptions.MapInboundClaims = false; // Set it false to obtain the original claim types from the token

        oidcOptions.TokenValidationParameters.NameClaimType = "name";
        oidcOptions.TokenValidationParameters.RoleClaimType = "role";

        var oidcEvents = new OpenIdConnectEvents
        {
            OnTicketReceived                       = onTicketReceived,
            OnRedirectToIdentityProviderForSignOut = onRedirectToIdentityProviderForSignOut
        };

        oidcOptions.Events = oidcEvents;
    }

    private static Task onRedirectToIdentityProviderForSignOut(RedirectContext context)
    {
        var oidcConfig = context.HttpContext.RequestServices.GetRequiredService<OidcConfig>();

        // string? returnToUrl = context.Properties.RedirectUri; // Ignore this value, as only the base URL is defined in Auth0 'Allowed Logout URLs'

        string returnToUrl = $"{context.Request.Scheme}://{context.Request.Host}";

        string logoutUri = $"{oidcConfig.Authority}/v2/logout?client_id={oidcConfig.ClientId}&returnTo={Uri.EscapeDataString(returnToUrl)}";

        context.Response.Redirect(logoutUri);

        context.HandleResponse();

        return Task.CompletedTask;
    }

    private static Task onTicketReceived(TicketReceivedContext context)
    {
        // This event can be used
        // 1) Replace the context.Principal by creating a new one
        // 2) Identify the user based on a claim (NameIdentifier) and create or retrieve the user from the database

        // context.HttpContext.RequestServices

        // context.Principal = transformClaims(context.Principal);

        Console.WriteLine("Ticket received event for '{0}'", context.Principal?.Identity?.Name);

        return Task.CompletedTask;
    }

    private static OidcConfig addOidcConfig(this IHostApplicationBuilder builder)
    {
        OidcConfig oidcConfig = builder.Configuration.GetSection("OidcConfig").Get<OidcConfig>()
            ?? throw new NullReferenceException("Missing 'OidcConfig' from configuration");

        builder.Services.AddSingleton(oidcConfig);

        return oidcConfig;
    }
}
