using BlazorWASM.BFF.Auth0.OpenIDConnect.Shared.Defaults;
using Microsoft.JSInterop;

namespace BlazorWASM.BFF.Auth0.OpenIDConnect.Client.Infrastructure;

public interface IAntiforgeryHttpClientFactory
{
    Task<HttpClient> CreateClientAsync(string clientName = AuthDefaults.AuthorizedClientName);
}

public sealed class AntiforgeryHttpClientFactory : IAntiforgeryHttpClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJSRuntime _jsRuntime;

    public AntiforgeryHttpClientFactory(IHttpClientFactory httpClientFactory, IJSRuntime jSRuntime)
    {
        _httpClientFactory = httpClientFactory;
        _jsRuntime         = jSRuntime;
    }

    public async Task<HttpClient> CreateClientAsync(string clientName = AuthDefaults.AuthorizedClientName)
    {
        string token = await _jsRuntime.InvokeAsync<string>("getAntiForgeryToken");

        HttpClient client = _httpClientFactory.CreateClient(clientName);

        client.DefaultRequestHeaders.Add(AntiforgeryDefaults.HeaderName, token);

        return client;
    }
}
