using BlazorWASM.Auth0.JWT.OpenIDConnect.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorWASM.Auth0.JWT.OpenIDConnect.Client;

public static class Program
{
    public static async Task Main(string[] args)
    {
        WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
        IServiceCollection services    = builder.Services;
        IConfiguration configuration   = builder.Configuration;

        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);

        OidcConfig oidcConfig = services.addOidcConfig(configuration);

        // Add services to the container
        {
            services.AddOidcAuthentication(options => configureOidcOptions(options, oidcConfig))
                    .AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();

            services.addDefaultHttpClient(baseAddress);

            services.AddAuthorizationCore();

            //services.AddApiAuthorization(); // This is for IdentityServer
        }

        await builder.Build().RunAsync();
    }

    private static void configureOidcOptions(RemoteAuthenticationOptions<OidcProviderOptions> options, OidcConfig oidcConfig)
    {
        OidcProviderOptions oidcOptions = options.ProviderOptions;

        oidcOptions.Authority    = oidcConfig.Authority;
        oidcOptions.ClientId     = oidcConfig.ClientId;
        oidcOptions.MetadataUrl  = oidcConfig.MetadataUrl;
        oidcOptions.ResponseType = "code";

        oidcOptions.DefaultScopes.Add("email"); // Default values: openid, profile

        oidcOptions.AdditionalProviderParameters.Add("audience", oidcConfig.Audience);
    }

    private static void addDefaultHttpClient(this IServiceCollection services, Uri baseAddress)
    {
        services.AddHttpClient("ServerAPI", client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        services.AddSingleton(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));
    }

    private static OidcConfig addOidcConfig(this IServiceCollection services, IConfiguration configuration)
    {
        OidcConfig? oidcConfig = configuration
            .GetSection(nameof(OidcConfig))
            .Get<OidcConfig>()
            ?? throw new NullReferenceException("OidcConfig was not found in appsettings.json");

        services.AddSingleton(oidcConfig);

        return oidcConfig;
    }
}
