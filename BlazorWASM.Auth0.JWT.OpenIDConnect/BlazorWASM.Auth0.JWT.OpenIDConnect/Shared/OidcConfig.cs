namespace BlazorWASM.Auth0.JWT.OpenIDConnect.Shared;

public sealed class OidcConfig
{
    // SPA = Singe Page Application

    /// <summary>
    /// SPA Domain, https://Domain
    /// </summary>
    public required string Authority { get; init; }

    /// <summary>
    /// SPA ClientId
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    /// SPA https://Domain/.well-known/openid-configuration
    /// </summary>
    public required string MetadataUrl { get; init; }

    /// <summary>
    /// Custom API identifier
    /// </summary>
    public required string Audience { get; init; }
}
