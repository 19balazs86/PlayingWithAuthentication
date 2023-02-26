namespace BlazorWASM.BFF.Auth0.OpenIDConnect.Client.Infrastructure;

public sealed class HostAuthenticationStateProvider : AuthenticationStateProvider
{
    private const string _authType = nameof(HostAuthenticationStateProvider);

    private static readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60);

    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;
    private readonly ILogger<HostAuthenticationStateProvider> _logger;

    private DateTime _userLastCheck     = DateTime.MinValue;
    private ClaimsPrincipal _cachedUser = new(new ClaimsIdentity());

    public HostAuthenticationStateProvider(
        NavigationManager navigationManager,
        HttpClient httpClient,
        ILogger<HostAuthenticationStateProvider> logger)
    {
        _navigationManager = navigationManager;
        _httpClient        = httpClient;
        _logger            = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal principal = await getUser();

        return new AuthenticationState(principal);
    }

    public async Task<bool> UserIsAuthenticatedAsync()
    {
        ClaimsPrincipal principal = await getUser();

        return principal?.Identity?.IsAuthenticated ?? false;
    }

    public void SignIn()
    {
        string encodedReturnUrl = Uri.EscapeDataString(_navigationManager.ToBaseRelativePath(_navigationManager.Uri));

        string loginUrl = _navigationManager.ToAbsoluteUri($"{AuthDefaults.LogInPath}?returnUrl={encodedReturnUrl}").ToString();

        _logger.LogInformation("Navigate to login page: '{LoginUrl}'", loginUrl);

        _navigationManager.NavigateTo(loginUrl, forceLoad: true);
    }

    private async ValueTask<ClaimsPrincipal> getUser()
    {
        var now = DateTime.Now;

        if (now < _userLastCheck + _userCacheRefreshInterval)
        {
            return _cachedUser;
        }

        _cachedUser = await fetchUser();

        _userLastCheck = now;

        return _cachedUser;
    }

    private async Task<ClaimsPrincipal> fetchUser()
    {
        UserInfo? user = null;

        try
        {
            user = await _httpClient.GetFromJsonAsync<UserInfo>(AuthDefaults.UserInfoPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Fetching user failed");
        }

        return user!.ToClaimsPrincipal(_authType);
    }
}
