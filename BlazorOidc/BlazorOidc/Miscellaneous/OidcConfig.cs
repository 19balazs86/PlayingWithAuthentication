namespace BlazorOidc.Miscellaneous;

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
    /// The SPA Client Secret is used to refresh the token by calling the token endpoint through the backchannel
    /// </summary>
    public required string ClientSecret { get; init; }

    /// <summary>
    /// SPA https://Domain/.well-known/openid-configuration
    /// </summary>
    public required string MetadataUrl { get; init; }

    /// <summary>
    /// Custom API identifier
    /// </summary>
    public required string Audience { get; init; }

    public string[] ValidIssuers()
    {
        string authority = Authority.EndsWith('/') ? Authority.TrimEnd('/') : Authority + '/';

        return [Authority, authority];
    }
}
