namespace BlazorWASM.BFF.Auth0.OpenIDConnect.Client.Infrastructure;

public sealed class AuthorizedHandler : DelegatingHandler
{
    private readonly HostAuthenticationStateProvider _authenticationStateProvider;

    public AuthorizedHandler(HostAuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        bool isAuthenticated = await _authenticationStateProvider.UserIsAuthenticatedAsync();

        HttpResponseMessage responseMessage = isAuthenticated ?
            await base.SendAsync(request, cancellationToken) :
            new HttpResponseMessage(HttpStatusCode.Unauthorized);

        if (responseMessage.StatusCode is 0 or HttpStatusCode.Unauthorized)
        {
            _authenticationStateProvider.SignIn();
        }

        return responseMessage;
    }
}
