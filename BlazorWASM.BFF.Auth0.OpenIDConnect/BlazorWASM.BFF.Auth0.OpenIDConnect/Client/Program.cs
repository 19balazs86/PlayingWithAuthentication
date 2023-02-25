using BlazorWASM.BFF.Auth0.OpenIDConnect.Client.Infrastructure;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorWASM.BFF.Auth0.OpenIDConnect.Client;

public static class Program
{
    public static async Task Main(string[] args)
    {
        WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
        IServiceCollection services    = builder.Services;

        var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);

        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Add services to the container
        {
            services.addAuthentication();

            services.addDefaultHttpClient(baseAddress);

            services.addAntiforgeryHttpClient(baseAddress);
        }

        await builder.Build().RunAsync();
    }

    private static void addDefaultHttpClient(this IServiceCollection services, Uri baseAddress)
    {
        const string defaultName = "default";

        services.AddHttpClient(defaultName, client => client.BaseAddress = baseAddress);

        services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(defaultName));
    }

    private static void addAntiforgeryHttpClient(this IServiceCollection services, Uri baseAddress)
    {
        services.AddTransient<AuthorizedHandler>();

        // This HttpClient will navigate unauthorized calls to the login page using the AuthorizedHandler and HostAuthenticationStateProvider
        // Set AllowAutoRedirect to false, otherwise throw exception and AuthorizedHandler will not work
        services
            .AddHttpClient(AuthDefaults.AuthorizedClientName, client => client.BaseAddress = baseAddress)
            .AddHttpMessageHandler<AuthorizedHandler>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });

        services.AddTransient<IAntiforgeryHttpClientFactory, AntiforgeryHttpClientFactory>();
    }

    private static void addAuthentication(this IServiceCollection services)
    {
        services.AddAuthorizationCore();

        services.AddSingleton<HostAuthenticationStateProvider>();

        services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<HostAuthenticationStateProvider>());
    }
}