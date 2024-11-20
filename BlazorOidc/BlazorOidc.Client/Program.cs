using BlazorOidc.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SharedLib;

namespace BlazorOidc.Client;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        IServiceCollection services = builder.Services;

        var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);

        // Add services to the container
        {
            services.AddAuthorizationCore()
                    .AddCascadingAuthenticationState()
                    .AddAuthenticationStateDeserialization();

            services.AddHttpClient<IWeatherForecaster, ClientWeatherForecaster>(httpClient => httpClient.BaseAddress = baseAddress);

            // Anti-forgery HttpClient might be handy
            // https://github.com/19balazs86/PlayingWithAuthentication/blob/main/BlazorWASM.BFF.Auth0.OpenIDConnect/BlazorWASM.BFF.Auth0.OpenIDConnect/Client/Infrastructure/AntiforgeryHttpClientFactory.cs
        }

        await builder.Build().RunAsync();
    }
}
