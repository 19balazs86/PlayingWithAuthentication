namespace BlazorWASM.BFF.Auth0.OpenIDConnect.Shared.Defaults;

public static class AntiforgeryDefaults
{
    public const string HeaderName = "X-XSRF-TOKEN";
    public const string CookieName = "__Host-X-XSRF-TOKEN";
}
